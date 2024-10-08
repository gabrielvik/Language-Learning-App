﻿using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace LanguageLearningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, UserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post(UserLoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getUser = await _userService.GetUserAsync(model.Username);
            if (getUser == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            if (!VerifyPasswordHash(model.Password, getUser.PasswordHash, getUser.PasswordSalt))
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            string token = CreateToken(getUser);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddDays(1),
                SameSite = SameSiteMode.None,
                Secure = true
            };

            Response.Cookies.Append("accessToken", token, cookieOptions);
            return Ok(new { AccessToken = token });
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post(UserRegistrationDTO model)
        {
            if (!IsValidUsername(model.Username))
                ModelState.AddModelError("Username", "Username must be between 5 and 20 characters.");

            var usernameIsTaken = await _userService.IsUsernameTakenAsync(model.Username);
            if (usernameIsTaken)
                ModelState.AddModelError("Username", "Username is already taken.");

            if (!IsValidEmail(model.Email))
                ModelState.AddModelError("Email", "Not a valid email address.");

            var emailIsTaken = await _userService.IsEmailTakenAsync(model.Email);
            if (emailIsTaken)
                ModelState.AddModelError("Email", "Email is already taken.");

            if (!IsValidPassword(model.Password))
                ModelState.AddModelError("Password", "Password must be at least 8 characters and contain 1 special character.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            string token = CreateToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddDays(1),
                SameSite = SameSiteMode.None,
                Secure = true
            };

            Response.Cookies.Append("accessToken", token, cookieOptions);

            var userAdded = await _userService.AddUserAsync(user);

            if (userAdded)
                return Ok(user);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add user.");
        }

        [Authorize]
        [HttpGet("userInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (usernameClaim != null)
            {
                string username = usernameClaim.Value;

                var user = await _userService.GetUserAsync(username);
                if (user != null)
                {
                    return Ok(new
                    {
                        user.Username,
                        user.Email,
                        user.LearningLanguage,
                        LearnedLessons = user.LearnedLessonsJson != null
                            ? JsonSerializer.Deserialize<Dictionary<int, List<int>>>(user.LearnedLessonsJson)
                            : null
                    });
                }
            }

            return BadRequest("User information not found.");
        }

        [Authorize]
        [HttpPost("updateLearningLanguage")]
        public async Task<IActionResult> UpdateLearningLanguage([FromBody] UpdateLearningLanguageDTO model)
        {
            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (usernameClaim != null)
            {
                string username = usernameClaim.Value;
                var user = await _userService.GetUserAsync(username);
                if (user != null)
                {
                    user.LearningLanguage = model.LearningLanguage;
                    var updateResult = await _userService.UpdateUserAsync(user);

                    if (updateResult)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update learning language.");
                    }
                }
            }

            return BadRequest("User information not found.");
        }


        private static bool IsValidEmail(string email) => email.Contains("@") && email.Contains(".");
        private static bool IsValidUsername(string username) => !string.IsNullOrEmpty(username) && username.Length > 5 && username.Length < 20;
        private static bool IsValidPassword(string password) => password.Length > 8 && password.Any(c => !char.IsLetterOrDigit(c));

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}

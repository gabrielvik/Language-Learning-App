using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LanguageLearningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static User user = new User();
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

            var user = await _userService.GetUserAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            var isAuthenticated = await _userService.AuthenticateUserAsync(model.Username, model.Password);
            if (!isAuthenticated)
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
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
                ModelState.AddModelError("Password", "Password must be at least 8 characthers and 1 special characther.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = model.Username;
            user.Email = model.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var userAdded = await _userService.AddUserAsync(user);

            if(userAdded)
                return Ok(user);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add user.");
        }


        private static bool IsValidEmail(string email) => email.Contains("@") && email.Contains(".");
        private static bool IsValidUsername(string username) => !string.IsNullOrEmpty(username) || username.Length > 5 || username.Length < 20;
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

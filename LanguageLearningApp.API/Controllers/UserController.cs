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

            var getUser = await _userService.GetUserAsync(model.Username);
            if (getUser == null)
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }
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

            user = getUser;

            string token = CreateToken(user);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Token.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.RefreshToken.Expires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken.Token = newRefreshToken.Token;
            user.RefreshToken.Created = newRefreshToken.Created;
            user.RefreshToken.Expires = newRefreshToken.Expires;
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

        [Authorize]
        [HttpGet("email")]
        public IActionResult GetUserEmail()
        {
            // Retrieve the user's claims from the HttpContext
            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (usernameClaim != null && emailClaim != null)
            {
                string username = usernameClaim.Value;
                string email = emailClaim.Value;

                return Ok(new { Username = username, Email = email });
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

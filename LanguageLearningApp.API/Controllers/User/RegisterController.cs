using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LanguageLearningApp.API.Controllers
{
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserService _userService; 

        public RegisterController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<IActionResult> Post(UserRegistrationDTO model)
        {
            if (!UserService.IsValidUsername(model.Username))
                ModelState.AddModelError("Username", "Username must be between 5 and 20 characters.");

            var usernameIsTaken = await _userService.IsUsernameTakenAsync(model.Username);
            if (usernameIsTaken)
                ModelState.AddModelError("Username", "Username is already taken.");

            if (!UserService.IsValidEmail(model.Email))
                ModelState.AddModelError("Email", "Not a valid email address.");

            var emailIsTaken = await _userService.IsEmailTakenAsync(model.Email);
            if (emailIsTaken)
                ModelState.AddModelError("Email", "Email is already taken.");

            if (!UserService.IsValidPassword(model.Password))
                ModelState.AddModelError("Password", "Password must be at least 8 characthers and 1 special characther.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userAdded = await _userService.AddUserAsync(model.Username, model.Password, model.Email);
            if (userAdded)
            {
                return Ok("User registered successfully!");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add user.");
            }
        }
    }
}

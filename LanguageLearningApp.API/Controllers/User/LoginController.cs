using Microsoft.AspNetCore.Mvc;
using LanguageLearningApp.API.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace LanguageLearningApp.API.Controllers.User
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly UserService _userService;

        public LoginController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
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

            var isAuthenticated = await _userService.AuthenticateUserAsync(model.Username, model.Password);
            if (!isAuthenticated)
            {
                ModelState.AddModelError("Username", "Invalid username or password.");
                return BadRequest(ModelState);
            }

            return Ok("Login successful!");
        }
    }
}

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

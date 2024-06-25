using Microsoft.AspNetCore.Mvc;
using LanguageLearningApp.Data.Entities; // Ensure to include this namespace

namespace LanguageLearningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        [HttpGet("lesson")]
        public IActionResult GetLesson(int id)
        {
            var lesson = Lessons.GetLesson(id);

            if (lesson != null)
            {
                return Ok(lesson); 
            }

            return BadRequest("Lesson not found.");
        }
    }
}

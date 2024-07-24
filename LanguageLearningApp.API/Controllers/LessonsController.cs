using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Chat;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LanguageLearningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly OpenAIAPI _openAiApi;

        public LessonsController(UserService userService)
        {
            _openAiApi = new OpenAIAPI("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");
            _userService = userService;
        }

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
        [Authorize]
        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateResponse([FromBody] EvaluationDTO request)
        {
            var lessons = Lessons.GetLesson(request.LessonId);
            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (lessons == null)
            {
                return BadRequest("Invalid lesson ID.");
            }

            if (request.StageId < 0 || request.StageId >= lessons.Count)
            {
                return BadRequest("Invalid stage ID.");
            }

            var stage = lessons[request.StageId];

            if (request.PromptId < 0 || request.PromptId >= stage.Prompt.Count)
            {
                return BadRequest("Invalid prompt ID.");
            }

            if (usernameClaim == null)
            {
                return BadRequest("User information not found.");
            }

            string username = usernameClaim.Value;
            var user = await _userService.GetUserAsync(username);
            if (user == null)
            {
                return BadRequest("User information not found.");
            }

            if (request.PromptId+1 == stage.Prompt.Count)
            {
                user.AddCompletedLesson(request.LessonId, request.StageId);
                await _userService.UpdateUserAsync(user);
            }

            var prompt = stage.Prompt[request.PromptId];

            var learningLanguage = user.LearningLanguage;
            var apiResult = await _openAiApi.Chat.CreateChatCompletionAsync(
                new ChatRequest
                {
                    Model = "gpt-4o-mini",
                    Messages = new[]
                    {
                new ChatMessage
                {
                    Role = ChatMessageRole.System,
                    Content = $"Evaluate the response: '{request.UserResponse}' to the prompt: '{prompt}' in the context of learning the language: '{learningLanguage}'. Your response should be a short feedback response, respond shortly what can be improved with regard to spelling and grammatical rules of the language, explain why something is correct or incorrect, etc. The response must be a translation of the prompt, and nothing else."
                }
                    }
                });

            var evaluationResult = apiResult.Choices[0].Message.Content;

            return Ok(evaluationResult);
        }


    }
}
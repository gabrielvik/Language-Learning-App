using Microsoft.AspNetCore.Mvc;
using LanguageLearningApp.Data.Entities;
using OpenAI_API;
using System.Threading.Tasks;
using OpenAI_API.Chat;

namespace LanguageLearningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly OpenAIAPI _openAiApi;

        public LessonsController()
        {
            _openAiApi = new OpenAIAPI("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");
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

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateResponse(EvaluationDTO request)
        {
            var lessons = Lessons.GetLesson(request.LessonId);

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

            var prompt = stage.Prompt[request.PromptId];

            var apiResult = await _openAiApi.Chat.CreateChatCompletionAsync(
                new ChatRequest
                {
                    Model = "gpt-3.5-turbo-0125",
                    Messages = new[]
                    {
                        new ChatMessage
                        {
                            Role = ChatMessageRole.System,
                            Content = $"Evaluate the user's response: '{request.UserResponse}' to the prompt: '{prompt}'"
                        }
                    }
                });

            // Assuming the response contains the evaluation result directly
            var evaluationResult = apiResult.Choices[0].Message.Content;

            return Ok(evaluationResult);
        }
    }

    public class EvaluationDTO
    {
        public int LessonId { get; set; }
        public int StageId { get; set; }
        public int PromptId { get; set; }
        public string UserResponse { get; set; }
    }
}
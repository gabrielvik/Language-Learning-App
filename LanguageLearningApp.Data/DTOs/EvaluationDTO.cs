namespace LanguageLearningApp.Data.DTOs
{
    public class EvaluationDTO
    {
        public int LessonId { get; set; }
        public int StageId { get; set; }
        public int PromptId { get; set; }
        public string UserResponse { get; set; }
    }
}

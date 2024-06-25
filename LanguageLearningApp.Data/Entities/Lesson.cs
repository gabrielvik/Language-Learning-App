namespace LanguageLearningApp.Data.Entities
{
    public class Lesson
    {
        public string Name { get; set; }
        public List<string> Prompt { get; set; }

        public Lesson(string name, List<string> prompt)
        {
            Name = name;
            Prompt = prompt;
        }
    }
}

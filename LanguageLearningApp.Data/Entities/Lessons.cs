using LanguageLearningApp.Data.Lessons;

namespace LanguageLearningApp.Data.Entities
{
    public static class Lessons
    {
        public static Dictionary<int, List<Lesson>> lessons = new Dictionary<int, List<Lesson>>();

        static Lessons()
        {
            InitializeLessons();
        }

        private static void InitializeLessons()
        {
            lessons.Add(1, Greetings.greetings);
        }

        public static List<Lesson>? GetLesson(int id)
        {
            return lessons.ContainsKey(id) ? lessons[id] : null;
        }
    }
}

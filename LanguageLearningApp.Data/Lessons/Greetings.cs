using Azure.Core;

namespace LanguageLearningApp.Data.Lessons
{
    public static class Greetings
    {
        public static List<Lesson> greetings = new List<Lesson>();

        static Greetings()
        {
            InitializeGreetings();
        }

        private static void InitializeGreetings()
        {
            greetings.Add(new Lesson("Basic Greetings", new List<string>
            {
                "Hello",
                "Hi",
                "Good morning",
                "Good afternoon",
                "Good evening"
            }));

            greetings.Add(new Lesson("Introducing Yourself", new List<string>
            {
                "My name is [Your Name]",
                "I am from [Your Country]",
                "Nice to meet you",
                "How are you?",
                "I am fine, thank you"
            }));

            greetings.Add(new Lesson("Asking Questions", new List<string>
            {
                "What is your name?",
                "Where are you from?",
                "How old are you?",
                "What do you do?",
                "Do you speak [Language]?"
            }));

            greetings.Add(new Lesson("Responding to Questions", new List<string>
            {
                "My name is [Your Name]",
                "I am from [Your Country]",
                "I am [Your Age] years old",
                "I am a [Your Profession]",
                "Yes, I speak [Language]",
                "No, I don't speak [Language]"
            }));
        }
    }
}

using Azure.Core;
using System.Collections.Generic;

namespace LanguageLearningApp.Data.Lessons
{
    public static class Numbers
    {
        public static List<Lesson> numbers = new List<Lesson>();

        static Numbers()
        {
            InitializeNumbers();
        }

        private static void InitializeNumbers()
        {
            numbers.Add(new Lesson("Basic Numbers", new List<string>
            {
                "One",
                "Two",
                "Three",
                "Four",
                "Five",
                "Six",
                "Seven",
                "Eight",
                "Nine",
                "Ten"
            }));

            numbers.Add(new Lesson("Teens", new List<string>
            {
                "Eleven",
                "Twelve",
                "Thirteen",
                "Fourteen",
                "Fifteen",
                "Sixteen",
                "Seventeen",
                "Eighteen",
                "Nineteen"
            }));

            numbers.Add(new Lesson("Tens", new List<string>
            {
                "Ten",
                "Twenty",
                "Thirty",
                "Forty",
                "Fifty",
                "Sixty",
                "Seventy",
                "Eighty",
                "Ninety",
                "One hundred"
            }));

            numbers.Add(new Lesson("Large Numbers", new List<string>
            {
                "One hundred",
                "One thousand",
                "Ten thousand",
                "One hundred thousand",
                "One million",
                "Ten million",
                "One hundred million",
                "One billion",
                "Ten billion",
                "One hundred billion"
            }));
        }
    }
}

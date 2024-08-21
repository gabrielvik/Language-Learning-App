using Azure.Core;
using System.Collections.Generic;

namespace LanguageLearningApp.Data.Lessons
{
    public static class Food
    {
        public static List<Lesson> foods = new List<Lesson>();

        static Food()
        {
            InitializeFoodTranslations();
        }

        private static void InitializeFoodTranslations()
        {
            foods.Add(new Lesson("Common foods", new List<string>
            {
                "Bread",
                "Cheese",
                "Rice",
                "Pasta",
                "Chicken",
                "Beef",
                "Fish",
                "Vegetables",
                "Fruits",
                "Eggs"
            }));

            foods.Add(new Lesson("Beverages", new List<string>
            {
                "Water",
                "Tea",
                "Coffee",
                "Juice",
                "Milk",
                "Wine",
                "Beer",
                "Soda",
                "Smoothie",
                "Hot chocolate"
            }));

            foods.Add(new Lesson("Dining Phrases", new List<string>
            {
                "I would like to order [Dish Name]",
                "Can I have the menu, please?",
                "Do you have any vegetarian options?",
                "I am allergic to [Ingredient]",
                "What is the special of the day?",
                "Can I get the bill, please?",
                "The food is delicious",
                "Can I get this to go?",
                "Can you recommend a dish?",
                "I would like a table for [Number of People]"
            }));

            foods.Add(new Lesson("Describing Taste", new List<string>
            {
                "This is salty",
                "This is sweet",
                "This is spicy",
                "This is bitter",
                "This is sour",
                "This is savory",
                "This is bland",
                "This is delicious",
                "This is too salty",
                "This is undercooked"
            }));
        }
    }
}

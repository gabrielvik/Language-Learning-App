using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LanguageLearningApp.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? LearningLanguage { get; set; }
        public string? LearnedLessonsJson { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public void AddCompletedLesson(int lessonId, int stageId)
        {
            var learnedLessons = string.IsNullOrEmpty(LearnedLessonsJson)
                ? new Dictionary<int, List<int>>()
                : JsonSerializer.Deserialize<Dictionary<int, List<int>>>(LearnedLessonsJson);

            if (learnedLessons.ContainsKey(lessonId))
            {
                if (!learnedLessons[lessonId].Contains(stageId))
                {
                    learnedLessons[lessonId].Add(stageId);
                }
            }
            else
            {
                learnedLessons[lessonId] = new List<int> { stageId };
            }

            LearnedLessonsJson = JsonSerializer.Serialize(learnedLessons);
        }
    }
}

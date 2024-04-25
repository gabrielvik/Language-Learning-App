﻿namespace LanguageLearningApp.Data.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }

        // Foreign key
        public int UserId { get; set; }
    }
}

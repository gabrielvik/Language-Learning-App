namespace LanguageLearningApp.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        // Navigation property for RefreshToken
        public RefreshToken RefreshToken { get; set; } = new RefreshToken();
    }
}

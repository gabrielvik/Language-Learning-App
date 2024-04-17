using LanguageLearningApp.Data.Entities;

namespace LanguageLearningApp.API.Services
{
    public class UserService
    {
        private readonly LanguageAppContext _dbContext; // Inject your database context here

        public UserService(LanguageAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddUserAsync(string username, string password, string email)
        {
            try
            {
                // Create a new User entity
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(password)
                };

                // Add the user to the database
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                return true; // User added successfully
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database errors)
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false; // Failed to add user
            }
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    // User not found
                    return false;
                }

                // Hash the entered password for comparison
                var hashedEnteredPassword = HashPassword(password);

                // Compare the hashes
                return user.PasswordHash == hashedEnteredPassword;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating user: {ex.Message}");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

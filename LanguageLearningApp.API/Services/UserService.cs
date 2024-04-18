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

        public async Task<User> GetUserAsync(string username)
        {
            try
            {
                // Find the user by username
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user: {ex.Message}");
                return null;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                // Retrieve all users from the database
                var users = await _dbContext.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users: {ex.Message}");
                return new List<User>(); // Return an empty list if an error occurs
            }
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            try
            {
                // Check if any user has the provided email
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user != null; // Returns true if email is already taken
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email: {ex.Message}");
                return true; // Treat as taken on error (for safety)
            }
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                return user != null; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking username: {ex.Message}");
                return true; 
            }
        }

        public static bool IsValidEmail(string email) => email.Contains("@") && email.Contains(".");
        public static bool IsValidUsername(string username) => !string.IsNullOrEmpty(username) || username.Length > 5 || username.Length < 20;
        public static bool IsValidPassword(string password) => password.Length > 8 && password.Any(c => !char.IsLetterOrDigit(c));

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

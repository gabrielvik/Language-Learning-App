using Azure.Core;
using LanguageLearningApp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace LanguageLearningApp.API.Services
{
    public class UserService
    {
        private readonly LanguageAppContext _dbContext; // Inject your database context here

        public UserService(LanguageAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                // Create a new User entity
                var newUser = new User
                {
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    PasswordSalt = user.PasswordSalt,
                    RefreshToken = user.RefreshToken
                };

                // Add the user to the database
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                return true; // User added successfully
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false; // Failed to add user
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
    }
}

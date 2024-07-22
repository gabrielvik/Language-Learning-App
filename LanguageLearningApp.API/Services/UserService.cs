using Azure.Core;
using LanguageLearningApp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LanguageLearningApp.API.Services
{
    public class UserService
    {
        private readonly LanguageAppContext _dbContext;

        public UserService(LanguageAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                var newUser = new User
                {
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    PasswordSalt = user.PasswordSalt,
                };

                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
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
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user != null;
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

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    // Update the user properties
                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    existingUser.LearningLanguage = user.LearningLanguage;
                    existingUser.PasswordHash = user.PasswordHash;
                    existingUser.PasswordSalt = user.PasswordSalt;
                    existingUser.LearnedLessonsJson = user.LearnedLessonsJson;

                    // Save changes to the database
                    _dbContext.Users.Update(existingUser);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
                return false; // User not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }
    }
}
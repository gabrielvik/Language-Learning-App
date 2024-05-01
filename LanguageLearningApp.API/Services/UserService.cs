using Azure.Core;
using LanguageLearningApp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<bool> UpdateRefreshToken(User user, RefreshToken refreshToken)
        {
            try
            {
                var dbUser = await _dbContext.Users
                    .Include(u => u.RefreshToken)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (dbUser != null)
                {
                    // Update the RefreshToken properties
                    dbUser.RefreshToken = refreshToken;

                    await _dbContext.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating refresh token: {ex.Message}");
                throw; 
            }
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
                    RefreshToken = user.RefreshToken
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

        public async Task<User>? InitializeUser(string refreshToken)
        {
            try
            {
                // Check if RefreshToken is expired
                var dbRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
                if (dbRefreshToken == null || dbRefreshToken.Expires < DateTime.Now)
                {
                    return null;
                }

                var dbUser = await _dbContext.Users.FirstOrDefaultAsync(r => r.Id == dbRefreshToken.UserId);
                return dbUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email: {ex.Message}");
                return null;
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
    }
}

using LanguageLearningApp.UI.Clients;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class UserService
{
    private readonly HttpClient _httpClient;

    public List<string> ErrorMessages { get; private set; } = new List<string>();

    public UserService(UserHttpClient userHttpClient)
    {
        _httpClient = userHttpClient.HttpClient;
    }

    public async Task<bool> RegisterUserAsync(UserRegistrationDTO model)
    {
        try
        {
            // Clear previous error messages
            ErrorMessages.Clear();

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ErrorMessages.Add("Please provide valid values for all required fields.");
                return false; // Registration failed due to empty fields
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("user/register", model);
            if (response.IsSuccessStatusCode)
            {
                return true; // Registration successful
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                // Parse error response and set error messages
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errorResponse);

                if (errorDictionary != null && errorDictionary.Count > 0)
                {
                    foreach (var errors in errorDictionary.Values)
                    {
                        ErrorMessages.AddRange(errors);
                    }
                }
            }

            return false; // Registration failed due to server error
        }
        catch (Exception ex)
        {
            ErrorMessages.Add("Failed to register user: " + ex.Message);
            return false; // Registration failed due to exception
        }
    }

    public async Task<bool> AuthenticateUserAsync(UserLoginDTO model)
    {
        try
        {
            // Clear previous error messages
            ErrorMessages.Clear();

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ErrorMessages.Add("Please provide both username/email and password.");
                return false; // Authentication failed due to empty fields
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("user/login", model);
            if (response.IsSuccessStatusCode)
            {
                return true; // Authentication successful
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Invalid username or password
                ErrorMessages.Add("Invalid username or password.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                // Parse error response and set error messages
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errorResponse);

                if (errorDictionary != null && errorDictionary.Count > 0)
                {
                    foreach (var errors in errorDictionary.Values)
                    {
                        ErrorMessages.AddRange(errors);
                    }
                }
            }

            return false; // Authentication failed
        }
        catch (Exception ex)
        {
            ErrorMessages.Add("Failed to authenticate user: " + ex.Message);
            return false; // Authentication failed due to exception
        }
    }
}

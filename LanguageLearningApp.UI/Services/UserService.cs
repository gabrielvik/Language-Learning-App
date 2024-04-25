using LanguageLearningApp.Data.Context;
using LanguageLearningApp.UI.Clients;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class UserService : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public User User { get; private set; }

    public List<string> ErrorMessages { get; private set; } = new List<string>();

    public UserService(UserHttpClient userHttpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = userHttpClient.HttpClient;
        _httpContextAccessor = httpContextAccessor;
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

    public string GetRefreshToken()
    {
		var context = _httpContextAccessor.HttpContext;
		return context?.Request.Cookies["accessToken"];
	}
    public async Task<string> Initialize()
    {
        var refreshToken = GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return "Refresh token is missing or invalid.";
        }

        // Set the Authorization header with the refresh token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);

        HttpResponseMessage response = await _httpClient.GetAsync("User/email");

        if (response.IsSuccessStatusCode)
        {
            string userInfo = await response.Content.ReadAsStringAsync();
            return userInfo;
        }
        else
        {
            return "Failed to retrieve user information. HTTP status code: " + response.StatusCode;
        }
    }

    public async Task<bool> AuthenticateUserAsync(UserLoginDTO model)
    {
        try
        {
            ErrorMessages.Clear();

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ErrorMessages.Add("Please provide both username/email and password.");
                return false; // Authentication failed due to empty fields
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("user/login", model);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
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
            return false;
        }
    }
}

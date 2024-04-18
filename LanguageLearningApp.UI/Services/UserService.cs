using LanguageLearningApp.UI.Clients;
using System.Text.Json;

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

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("register", model);
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
}

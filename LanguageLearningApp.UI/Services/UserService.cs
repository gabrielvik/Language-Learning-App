using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LanguageLearningApp.Data.DTOs;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RegisterUserAsync(UserRegistrationDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("register", model); // Endpoint should be relative

        return response.IsSuccessStatusCode;
    }

}
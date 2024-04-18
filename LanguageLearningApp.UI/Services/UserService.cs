using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LanguageLearningApp.Data.DTOs;
using LanguageLearningApp.UI.Clients;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(UserHttpClient userHttpClient)
    {
        _httpClient = userHttpClient.HttpClient;
    }

    public async Task<bool> RegisterUserAsync(UserRegistrationDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("register", model);

        return response.IsSuccessStatusCode;
    }
}
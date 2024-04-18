namespace LanguageLearningApp.UI.Clients
{
    public class UserHttpClient
    {
        public HttpClient HttpClient { get; }

        public UserHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            HttpClient.BaseAddress = new Uri("https://localhost:7134/api/");
        }
    }
}

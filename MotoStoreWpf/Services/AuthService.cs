using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MotoStoreWpf.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44335/api/Auth";
        public string? CurrentUserName { get; private set; }

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginData = new
            {
                Username = username,
                PasswordHash = password
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/login/", content);

            if (response.IsSuccessStatusCode)
            {
                CurrentUserName = username;
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var registerData = new
            {
                Username = username,
                Password = password
            };

            var json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/register", content);
            return response.IsSuccessStatusCode;
        }
    }
}

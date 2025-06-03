using MotoStore.Api.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MotoStoreWpf.Services
{
    class OrderService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44335/api";

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateOrderAsync(Order order)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/orders/with-items/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}

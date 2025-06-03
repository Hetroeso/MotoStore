using MotoStore.Api.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MotoStoreWpf.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44335/api/products";

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>(BaseUrl);
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товаров: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{product.Id}", product);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении товара: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/", product);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{productId}");
            return response.IsSuccessStatusCode;
        }
    }
}

using System.Text;
using System.Text.Json;
using TaskManager.Web.Interfaces.Category;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(HttpClient httpClient, ILogger<CategoryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            try
            {
                // Ajusta la ruta "api/categories" según tu Backend real
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/categories");
                return categories ?? new List<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categorías.");
                return new List<CategoryViewModel>();
            }
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CategoryViewModel>($"api/categories/{id}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> CreateAsync(CategoryViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/categories", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, CategoryViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/categories/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/categories/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
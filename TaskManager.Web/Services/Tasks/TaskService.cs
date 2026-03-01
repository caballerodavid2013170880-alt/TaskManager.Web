using System.Text.Json;
using System.Text;
using TaskManager.Web.Interfaces.Task;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services.Tasks
{
    // Implementación ÚNICA: Maneja la lógica y la llamada HTTP
    public class TaskService : ITaskService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaskService> _logger;

        // Inyectamos HttpClient configurado en Program.cs
        public TaskService(HttpClient httpClient, ILogger<TaskService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<TaskViewModel>> GetAllAsync(string searchTerm = null)
        {
            try
            {
                // Construimos la URL con query params si existe búsqueda
                var url = "api/taskitems"; // Asumiendo que esta es tu ruta base en la API
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    url += $"?searchTerm={Uri.EscapeDataString(searchTerm)}";
                }

                // Usamos GetFromJsonAsync para simplificar la deserialización
                var tasks = await _httpClient.GetFromJsonAsync<List<TaskViewModel>>(url);
                return tasks ?? new List<TaskViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo tareas.");
                return new List<TaskViewModel>(); // Retorna lista vacía en error para no romper la vista
            }
        }

        public async Task<TaskViewModel?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TaskViewModel>($"api/taskitems/{id}");
            }
            catch (HttpRequestException)
            {
                return null; // Si no existe o falla, retornamos null
            }
        }

        public async Task<bool> CreateAsync(CreateTaskViewModel model)
        {
            // Regla de Negocio Frontend: Validación extra antes de enviar (opcional)
            if (model.DueDate < DateTime.Today) return false;

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/taskitems", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, EditTaskViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/taskitems/{id}", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/taskitems/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
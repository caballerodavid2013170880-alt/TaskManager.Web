using Microsoft.AspNetCore.Http;
using TaskManager.Web.Models;
using TaskManager.Web.Utilities.Exceptions;

namespace TaskManager.Web.Services.Business
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryApiClient _categoryApiClient;

        public CategoryService(ICategoryApiClient categoryApiClient)
        {
            _categoryApiClient = categoryApiClient;
        }

        public async Task<string> ImportFromExcelAsync(IFormFile file)
        {
            // Centra la validación básica de protección
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Debe seleccionar un archivo Excel.");
            }

            // Llama al cliente de API y retorna el mensaje procesado
            return await _categoryApiClient.ImportCategoriesFromExcelAsync(file);
        }

        public async Task<IEnumerable<object>> GetCategoriesAsync()
        {
            // Queda pendiente para otra clase
            return await _categoryApiClient.GetAllCategoriesAsync();
        }

        //250226
        public async Task<List<CategoryOptionViewModel>> GetSimpleListAsync()
        {
            // Llamamos al endpoint de la API: /api/categories/simple-list
            //var response = await _httpClient.GetAsync("/api/categories");
            return await _categoryApiClient.GetSimpleListAsync();

            /*
             * if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var message = string.IsNullOrWhiteSpace(body)
                    ? $"Error al obtener categorías. Código: {(int)response.StatusCode}"
                    : body;

                throw new ApiException(message, (int)response.StatusCode);
            }
            

            var categories =
                await response.Content.ReadFromJsonAsync<List<CategoryOptionViewModel>>()
                ?? new List<CategoryOptionViewModel>();
            
            return categories;
             */
        }

    }


}
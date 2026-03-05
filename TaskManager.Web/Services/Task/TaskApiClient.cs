using System.Net.Http.Json;
using TaskManager.Web.Models;
using System.Net.Http.Headers;

namespace TaskManager.Web.Services
{
    public class TaskApiClient : ITaskApiClient
    {
        private readonly HttpClient _httpClient;


        public TaskApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<PagedResultViewModel<TaskViewModel>> GetTasksAsync(int page = 1, int pageSize = 5)
        {
            var url = $"/api/tasks/advanced-search?page={page}&pageSize={pageSize}";

            //var result = await _httpClient.GetFromJsonAsync<PagedResultViewModel<TaskViewModel>>(url);
            //GCorreciones
            return await _httpClient.GetFromJsonAsync<PagedResultViewModel<TaskViewModel>>(url) ?? new PagedResultViewModel<TaskViewModel>();
        }

        public async Task<PagedResultViewModel<TaskViewModel>> SearchTasksAsync(TaskSearchViewModel filters)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(filters.Text))
                query.Add($"text={filters.Text}");

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
                query.Add($"categoryName={filters.CategoryName}");

            if (filters.IsCompleted.HasValue)
                query.Add($"isCompleted={filters.IsCompleted.Value}");

            if (filters.Step.HasValue)
                query.Add($"step={filters.Step.Value}");

            query.Add($"page={filters.Page}");
            query.Add($"pageSize={filters.PageSize}");

            var finalQueryString = string.Join("&", query);

            var url = $"/api/tasks/advanced-search?{finalQueryString}";
            //GCorrecciones advertencias de nulidad
            return await _httpClient.GetFromJsonAsync<PagedResultViewModel<TaskViewModel>>(url)
           ?? new PagedResultViewModel<TaskViewModel>();
        }


        // DIa 3 220126
        public async Task<bool> CreateTaskAsync(CreateTaskViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/tasks",
                model
            );

            response.EnsureSuccessStatusCode(); //Lanza Exception si no es exitosa
            return true;
        }
        
        //270126
        //GET
        public async Task<EditTaskViewModel> GetTaskByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/tasks/{id}");

            if (response == null) throw new Exception("No se encontró la tarea");

            return new EditTaskViewModel
            {
                Id = response.Id,
                Title = response.Title,
                CategoryId = response.CategoryId,
                Step = response.Step,
                IsCompleted = response.IsCompleted
            };
        }

        /*180226
        public async Task<EditTaskViewModel> GetTaskById2Async(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/tasks/{id}");

            if (response == null) throw new Exception("No se encontró la tarea");

            return new EditTaskViewModel
            {
                Id = response.Id,
                Title = response.Title,
                CategoryId = response.CategoryId,
                Step = response.Step,
                IsCompleted = response.IsCompleted
            };
        }
        */
        //270126
        //POST
        public async Task UpdateTaskAsync(EditTaskViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/api/tasks/{model.Id}",
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                // Leer mensaje de la API
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }
        // Sesión 280126 Si la tarea esta en IsCompleted=true se puede borrar?
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/tasks/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return true;
        }
        //Sollución error 2 030226 compilación
        /*
        public Task<TaskViewModel> GetTaskDetailAsync(int id)
        {
            throw new NotImplementedException();
        }

        */
        public async Task<TaskViewModel> GetTaskDetailAsync(int id)
        {
            // Realiza la petición a la API para obtener una sola tarea por su ID
            var result = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/tasks/{id}");

            // Si la API devuelve nulo, lanzamos una excepción o manejamos el error
            if (result == null)
            {
                throw new Exception("No se pudieron obtener los detalles de la tarea.");
            }

            return result;
        }


        //060226    Import Task en Web
        public async Task<string> ImportTasksFromExcelAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();

            // Convertimos el IFormFile en StreamContent
            using var stream = file.OpenReadStream(); //Crea un espacio virtual
            var fileContent = new StreamContent(stream); //Otorga accesos para editar el archivo

            // Tipo MIME típico para Excel .xlsx (no es obligatorio pero está bien ponerlo)
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            // El "file" aquí debe coincidir con el nombre del parámetro en el endpoint de la API
            content.Add(fileContent, "file", file.FileName);

            // Llamamos a la API
            var response = await _httpClient.PostAsync("/api/tasks/import-excel", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al importar tareas. Respuesta API: {errorBody}");
            }

            // Leemos el JSON que envía la API
            var result = await response.Content.ReadFromJsonAsync<ImportTaskResult>();

            // Si por alguna razón no se pudo deserializar
            if (result == null || string.IsNullOrWhiteSpace(result.Message))
            {
                return "Importación realizada correctamente.";
            }

            // Devolvemos el mensaje que vino de la API
            // Ej: "Se importaron 5 tareas nuevas."
            return result.Message +
                   (result.Duplicadas > 0
                        ? $" ({result.Duplicadas} filas duplicadas no se importaron.)"
                        : string.Empty);
        }
        // 060226    Import Task en Web




        //040626
        public async Task<PagedResultViewModel<TaskViewModel>> AdvancedSearchAsync(TaskSearchViewModel filters)
        {
            var query = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(filters.Text))
                query["text"] = filters.Text; //asignación directa

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
                query["categoryName"] = filters.CategoryName;

            if (filters.CategoryId.HasValue)
                query["categoryId"] = filters.CategoryId.Value.ToString();//conversión por tipado requerido

            if (filters.Step.HasValue)
                query["step"] = filters.Step.Value.ToString();

            if (filters.IsCompleted.HasValue)
                query["isCompleted"] = filters.IsCompleted.Value.ToString().ToLower();

            query["page"] = filters.Page.ToString();
            query["pageSize"] = filters.PageSize.ToString();

            // Construir una URL con QueryString dinámico
            var queryString = string.Join("&",
                query.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            var url = $"/api/tasks/advanced-search?{queryString}";

            return await _httpClient.GetFromJsonAsync<PagedResultViewModel<TaskViewModel>>(url)
                   ?? new PagedResultViewModel<TaskViewModel> // ?? ops de coalescencia nula: asignar un valor predeterminado si es null
                   {
                       Items = new List<TaskViewModel>(),
                       Page = filters.Page,
                       PageSize = filters.PageSize,
                       TotalCount = 0
                   };
        }
    }
}
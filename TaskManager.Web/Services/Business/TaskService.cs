using TaskManager.Web.Models;

namespace TaskManager.Web.Services.Business
{
    public class TaskService : ITaskService
    {
        private readonly ITaskApiClient _client;

        public TaskService(ITaskApiClient client)
        {
            _client = client;
        }

        // Arreglo DCC improvisado 230126
        public async Task<TaskSearchViewModel> GetIndexModelAsync(int page, int pageSize)
        {
            return new TaskSearchViewModel
            {
                Page = page,
                PageSize = pageSize,
                Result = await _client.GetTasksAsync(page, pageSize)
            };
        }

        public async Task<TaskSearchViewModel> SearchTasksAsync(TaskSearchViewModel model)
        {
            if (model.Page == 0) model.Page = 1;
            model.Result = await _client.SearchTasksAsync(model);
            return model;
        }

        public async Task<bool> CreateTaskAsync(CreateTaskViewModel model)
        {
            return await _client.CreateTaskAsync(model);
        }

        // Versiones con mensaje UX por falta de ID
        public async Task<EditTaskViewModel> GetTaskForEditAsync(int? id)
        {
            if (id == null || id == 0) return null;
            return await _client.GetTaskByIdAsync(id.Value);
        }

        public async Task<EditTaskViewModel> GetTaskByIdAsync(int id)
        {
            // SL le pide al ApiClient que busque la tarea
            // _client es el campo inyectado en el constructor
            return await _client.GetTaskByIdAsync(id);
        }

        public async Task UpdateTaskAsync(EditTaskViewModel model)
        {
            await _client.UpdateTaskAsync(model);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            return await _client.DeleteTaskAsync(id);
        }

        public async Task<TaskViewModel> GetDetailsAsync(int? id)
        {
            if (id == null || id == 0) return null;
            return await _client.GetTaskDetailAsync(id.Value);
        }

        // 060326 Import Excel tareas
        public async Task<string> ImportFromExcelAsync(IFormFile file)
        {
            return await _client.ImportTasksFromExcelAsync(file);
        }

        // 040226 Advanced Search
        public async Task<TaskSearchViewModel> GetAdvancedSearchModelAsync(TaskSearchViewModel filters)
        {
            filters.Result = await _client.AdvancedSearchAsync(filters);
            return filters;
        }

        public async Task<TaskSearchViewModel> AdvancedSearchAsync(TaskSearchViewModel filters)
        {
            filters.Result = await _client.AdvancedSearchAsync(filters);
            return filters;
        }


    }
}
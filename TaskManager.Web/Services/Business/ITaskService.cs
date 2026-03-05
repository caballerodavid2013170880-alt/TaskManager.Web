using Microsoft.AspNetCore.Http;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services.Business
{
    public interface ITaskService
    {
        // Métodos existentes en tu controlador
        Task<TaskSearchViewModel> GetIndexModelAsync(int page, int pageSize);
        Task<TaskSearchViewModel> SearchTasksAsync(TaskSearchViewModel model);
        Task<bool> CreateTaskAsync(CreateTaskViewModel model);
        Task<EditTaskViewModel> GetTaskForEditAsync(int? id);

        // Dentro de ITaskService.cs 180826 por task-modal.js
        Task<EditTaskViewModel> GetTaskByIdAsync(int id);
        Task UpdateTaskAsync(EditTaskViewModel model);
        Task<bool> DeleteTaskAsync(int id);
        Task<TaskViewModel> GetDetailsAsync(int? id);

        // 060326 Import Excel tareas
        Task<string> ImportFromExcelAsync(IFormFile file);

        // 040226 Búsqueda avanzada
        Task<TaskSearchViewModel> GetAdvancedSearchModelAsync(TaskSearchViewModel filters);

        Task<TaskSearchViewModel> AdvancedSearchAsync(TaskSearchViewModel filters);
    }
}
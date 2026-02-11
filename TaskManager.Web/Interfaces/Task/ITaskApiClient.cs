using System.Net.Http;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public interface ITaskApiClient
    {
        Task<PagedResultViewModel<TaskViewModel>> GetTasksAsync(int page = 1, int pageSize = 5);
        Task<PagedResultViewModel<TaskViewModel>> SearchTasksAsync(TaskSearchViewModel filters);
        Task <bool> CreateTaskAsync(CreateTaskViewModel model);
        Task<EditTaskViewModel> GetTaskByIdAsync(int id);
        Task UpdateTaskAsync(EditTaskViewModel model);

        Task<bool> DeleteTaskAsync(int id); //280126    

        Task<TaskViewModel> GetTaskDetailAsync(int id); //300126

        Task<string> ImportTasksFromExcelAsync(IFormFile file);

        Task<PagedResultViewModel<TaskViewModel>> AdvancedSearchAsync(TaskSearchViewModel filters);


    }

}
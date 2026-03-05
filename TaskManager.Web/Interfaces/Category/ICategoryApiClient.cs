using Microsoft.AspNetCore.Http;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public interface ICategoryApiClient
    {
        Task<string> ImportCategoriesFromExcelAsync(IFormFile file);

        Task<IEnumerable<dynamic>> GetAllCategoriesAsync();
        Task<List<CategoryOptionViewModel>> GetSimpleListAsync();
    }
}
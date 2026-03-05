using Microsoft.AspNetCore.Http;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services.Business
{
    public interface ICategoryService
    {
        // 060326 Import Excel categorías
        Task<string> ImportFromExcelAsync(IFormFile file);

        // Queda pendiente para implementar lógica de Index en el futuro
        Task<IEnumerable<object>> GetCategoriesAsync();
        Task<List<CategoryOptionViewModel>> GetSimpleListAsync();
    }
}
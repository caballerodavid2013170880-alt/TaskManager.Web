using TaskManager.Web.Models;

namespace TaskManager.Web.Interfaces.Category
{
    public interface ICategoryService
    {
        // 📋 Lectura
        Task<List<CategoryViewModel>> GetAllAsync(); // Vital para el Dropdown de Tareas
        Task<CategoryViewModel?> GetByIdAsync(int id);

        // ✍️ Escritura (CRUD)
        Task<bool> CreateAsync(CategoryViewModel model);
        Task<bool> UpdateAsync(int id, CategoryViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
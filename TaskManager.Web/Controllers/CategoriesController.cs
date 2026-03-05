using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
<<<<<<< Updated upstream
using TaskManager.Web.Services;
=======
using TaskManager.Web.Interfaces.Category;
using TaskManager.Web.Models;
>>>>>>> Stashed changes
=======
using TaskManager.Web.Services.Business; // Importante añadir esta referencia
>>>>>>> tras-status-quo

namespace TaskManager.Web.Controllers
{
    public class CategoriesController : Controller
    {
<<<<<<< HEAD
<<<<<<< Updated upstream
        private readonly ICategoryApiClient _categoryApiClient;
=======
        // Inyección de Dependencia ÚNICA
        private readonly ICategoryService _categoryService;
>>>>>>> Stashed changes
=======
        // Ahora inyectamos el SERVICIO en lugar del CLIENTE
        private readonly ICategoryService _categoryService;
>>>>>>> tras-status-quo

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel category)
        {
<<<<<<< HEAD
<<<<<<< Updated upstream
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo Excel.";
                return View();
            }

=======
>>>>>>> tras-status-quo
            try
            {
                // El controlador solo delega la tarea al servicio
                var resultMessage = await _categoryService.ImportFromExcelAsync(file);
                TempData["Success"] = resultMessage;
            }
            catch (ArgumentException ex)
            {
                // Errores de validación conocidos
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                // Errores inesperados de la API
                TempData["Error"] = "Ocurrió un error al importar el archivo: " + ex.Message;
            }

            return View();
=======
            if (ModelState.IsValid)
            {
                var result = await _categoryService.CreateAsync(category);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo crear la categoría. Intente nuevamente.");
            }
            return View(category);
>>>>>>> Stashed changes
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }
<<<<<<< HEAD
<<<<<<< Updated upstream
=======

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateAsync(id, category);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar la categoría.");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
>>>>>>> Stashed changes
=======

        [HttpGet]
        public async Task<JsonResult> GetCategoriesJson()
        {
            // Supongamos que tu cliente de categorías tiene un método para listar
            var categories = await _categoryService.GetCategoriesAsync();
            return Json(categories);
        }

        //250226
        // GET: /Categories/Options
        [HttpGet]
        public async Task<IActionResult> Options()
        {
            var categories = await _categoryService.GetSimpleListAsync();
            return Json(categories); // Devuelve JSON al JS del front
        }
>>>>>>> tras-status-quo
    }
}
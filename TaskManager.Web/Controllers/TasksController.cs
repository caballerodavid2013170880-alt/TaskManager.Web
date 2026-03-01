using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Web.Interfaces.Category; // Necesario para cargar las categorías en los select
using TaskManager.Web.Interfaces.Task;     // Nuestra nueva interfaz unificada
using TaskManager.Web.Models;
<<<<<<< Updated upstream
using TaskManager.Web.Services;
=======
>>>>>>> Stashed changes

namespace TaskManager.Web.Controllers
{
    public class TasksController : Controller
    {
<<<<<<< Updated upstream
        private readonly ITaskApiClient _client;

        public TasksController(ITaskApiClient client)
        {
            _client = client;
        }
        /*
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var result = await _client.GetTasksAsync(page, pageSize); //Retorna vista
            return View(result);    //En la vista retorna el resultado
        }*/
        //Arreglo DCC improvisado 230126
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var model = new TaskSearchViewModel
            {
                Page = page,
                PageSize = pageSize,
                Result = await _client.GetTasksAsync(page, pageSize)
            };

            return View(model);
        }


        public async Task<IActionResult> Search(TaskSearchViewModel model)
        {
            // Si es la primera carga de la página
            if (model.Page == 0)
                model.Page = 1;

            model.Result = await _client.SearchTasksAsync(model);

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateTaskViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _client.CreateTaskAsync(model);

            return RedirectToAction(nameof(Index));
        }
        //Se comenta versión original por modificación sin ID
        /*
        [HttpGet] 
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _client.GetTaskByIdAsync(id);
            return View(model);
        }
        */
        [HttpPost] 
        public async Task<IActionResult> Edit(EditTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _client.UpdateTaskAsync(model);
                TempData["Success"] = "La tarea fue actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost] 
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _client.DeleteTaskAsync(id);
                TempData["Success"] = "La tarea fue eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar la tarea: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        //Se comenta versión original por modificación sin ID
        /*
        //300126 Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _client.GetTaskDetailAsync(id);

            if (task == null)
            {
                TempData["Error"] = "La tarea no existe.";
                return RedirectToAction("Index");
            }

            return View(task);
        }
        */
        //Versiones con mensaje UX por falta de ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["Error"] = "Por favor indica el registro que quieres ver detalles.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _client.GetTaskDetailAsync(id.Value);
            if (task == null) return NotFound();

            return View(task);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["Error"] = "Por favor indica el registro que quieres editar.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _client.GetTaskByIdAsync(id.Value);
            if (task == null) return NotFound();

            return View(task);
        }


        //060326 Import Excel tareas
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo Excel.";
                return View();
            }

            try
            {
                var resultMessage = await _client.ImportTasksFromExcelAsync(file);
                TempData["Success"] = resultMessage;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al importar el archivo: " + ex.Message;
            }

            return View();
        }


        //060326 FIN Import Excel tareas


        
        // 040226
        [HttpGet]
        public async Task<IActionResult> Index2(TaskSearchViewModel filters)
        {
            var result = await _client.AdvancedSearchAsync(filters);
            filters.Result = result;
            return View(filters); // regresamos siempre el modelo completo
        }
        


        //050226 Ajax
        [HttpGet]
        public IActionResult AjaxDemo()
        {
            return View();
        }

=======
        // 🏗️ ARQUITECTURA: Inyección de Dependencias
        // Aquí pedimos las herramientas que necesitamos.
        // Ya no pedimos "ApiClient" y "Service" por separado. Solo el Service.
        private readonly ITaskService _taskService;
        private readonly ICategoryService _categoryService;

        public TasksController(ITaskService taskService, ICategoryService categoryService)
        {
            _taskService = taskService;
            _categoryService = categoryService;
        }

        // ============================================================
        // 🌍 VISTAS DE PÁGINA COMPLETA (Navegación tradicional)
        // ============================================================

        /// <summary>
        /// 🏠 INDEX: La entrada principal.
        /// Carga la página completa con el marco (Layout), cabeceras y pies de página.
        /// Llama al servicio para obtener la lista inicial de tareas.
        /// </summary>
        public async Task<IActionResult> Index(string searchTerm)
        {
            // Paso 1: Usar el servicio unificado para ir a buscar datos a la API
            var tasks = await _taskService.GetAllAsync(searchTerm);

            // Paso 2: Entregar la vista completa al usuario
            return View(tasks);
        }

        /// <summary>
        /// 🔍 DETAILS: Ver el detalle de una sola tarea.
        /// Navegación tradicional (cambia de URL).
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        // ============================================================
        // ⚡ MÉTODOS AJAX / PARCIALES (La magia de dev-sesiones)
        // Estos métodos NO recargan la página, solo devuelven trozos de HTML
        // ============================================================

        /// <summary>
        /// 🔄 TASK TABLE PARTIAL: Actualización dinámica de la tabla.
        /// Cuando el usuario busca o filtra, JS llama a este método.
        /// NO devuelve la página entera, solo el HTML de la tabla (`_TaskTablePartial`).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TaskTablePartial(string searchTerm)
        {
            // 1. Buscamos los datos filtrados
            var tasks = await _taskService.GetAllAsync(searchTerm);

            // 2. Devolvemos SOLO el trozo de la tabla HTML
            return PartialView("_TaskTablePartial", tasks);
        }

        /// <summary>
        /// ➕ CREATE (GET - Modal): El formulario vacío.
        /// JS llama aquí para obtener el HTML del formulario y mostrarlo en una ventana flotante.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateTaskPartial()
        {
            // Preparamos los datos necesarios para el formulario (lista de categorías)
            await LoadCategoriesViewBag();

            // Devolvemos el formulario vacío como vista parcial
            return PartialView("_TaskFormPartial", new CreateTaskViewModel());
        }

        /// <summary>
        /// 💾 CREATE (POST - Modal): Guardar desde el modal.
        /// Recibe los datos del formulario AJAX.
        /// Si todo sale bien, devuelve un JSON OK. Si falla, devuelve el formulario con errores.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTaskPartial(CreateTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _taskService.CreateAsync(model);
                if (result)
                {
                    // Éxito: Avisamos a JS para que cierre el modal y recargue la tabla
                    return Json(new { success = true, message = "Tarea creada correctamente" });
                }
                ModelState.AddModelError("", "Error al crear la tarea en la API.");
            }

            // Fallo: Recargamos categorías y devolvemos el formulario con los mensajes de error
            await LoadCategoriesViewBag();
            return PartialView("_TaskFormPartial", model);
        }

        /// <summary>
        /// ✏️ EDIT (GET - Modal): Cargar formulario con datos.
        /// JS llama aquí con el ID, buscamos la tarea y devolvemos el formulario relleno.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditTaskPartial(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();

            // Mapeamos de ViewModel de Lectura a ViewModel de Edición
            var editModel = new EditTaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                CategoryId = task.CategoryId // Asegurarse de tener esto en el modelo
            };

            await LoadCategoriesViewBag();
            return PartialView("_TaskFormPartial", editModel); // Reutilizamos el mismo formulario
        }

        /// <summary>
        /// 💾 EDIT (POST - Modal): Guardar cambios.
        /// Similar al Create, pero actualizando.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditTaskPartial(EditTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _taskService.UpdateAsync(model.Id, model);
                if (result)
                {
                    return Json(new { success = true, message = "Tarea actualizada correctamente" });
                }
                ModelState.AddModelError("", "Error al actualizar la tarea.");
            }

            await LoadCategoriesViewBag();
            return PartialView("_TaskFormPartial", model);
        }


        // ============================================================
        // 🗑️ MÉTODOS DE BORRADO (CRUD Standard)
        // ============================================================

        public async Task<IActionResult> Delete(int id)
        {
            // Muestra la página de confirmación "¿Estás seguro?"
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ejecuta el borrado real
            await _taskService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 🧪 MÉTODOS AUXILIARES / LEGACY / DEMOS
        // Se mantienen por compatibilidad con tus archivos actuales
        // ============================================================

        public IActionResult AjaxDemo()
        {
            return View();
        }

        // Vistas alternativas de Index que tenías en el proyecto
        public async Task<IActionResult> Index1(string searchTerm)
        {
            var tasks = await _taskService.GetAllAsync(searchTerm);
            return View(tasks);
        }

        public async Task<IActionResult> Index2(string searchTerm)
        {
            var tasks = await _taskService.GetAllAsync(searchTerm);
            return View(tasks);
        }

        /// <summary>
        /// 🛠️ Helper: Carga las categorías en el ViewBag.
        /// Se usa tanto en Create como en Edit para llenar el <select>.
        /// </summary>
        private async Task LoadCategoriesViewBag()
        {
            // Nota: Aquí asumimos que también migraste CategoryService a la nueva arquitectura
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
>>>>>>> Stashed changes
    }
}
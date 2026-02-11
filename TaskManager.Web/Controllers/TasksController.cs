using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.Models;
using TaskManager.Web.Services;

namespace TaskManager.Web.Controllers
{
    public class TasksController : Controller
    {
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

    }
}
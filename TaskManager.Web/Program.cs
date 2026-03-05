using TaskManager.Web.Extensions;
using TaskManager.Web.Interfaces.Category;
using TaskManager.Web.Interfaces.Task;
using TaskManager.Web.Services.Categories;
using TaskManager.Web.Services.Tasks;

var builder = WebApplication.CreateBuilder(args);
// CONFIGURACIÓN DE SERVICIOS (Service Layer Única)

// 1. Configurar HttpClient para TaskService
builder.Services.AddHttpClient<ITaskService, TaskService>(client =>
{
    // AJUSTA ESTO AL PUERTO REAL DE TU API (Backend)
    client.BaseAddress = new Uri("http://localhost:7125/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 2. Repite el patrón para Categorías (cuando hagas la migración de ese también)
builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
    // AJUSTA ESTO AL PUERTO REAL DE TU API (Backend)
    client.BaseAddress = new Uri("http://localhost:7125/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApiClients();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Aquí va el middleware MVC de errores
app.UseMvcGlobalErrorHandler();

app.UseAuthorization();

//Esa línea define la ruta “clásica” (MVC) que ASP.NET Core usa para decidir qué controlador y qué acción ejecutar cuando llega una petición HTTP. Si llega una URL y no coincide con nada más específico,intenta interpretarla como:Controlador / Acción / Id opcional.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.Run();

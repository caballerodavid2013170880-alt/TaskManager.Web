using TaskManager.Web.Http;
using TaskManager.Web.Services;
using TaskManager.Web.Services.Business;

namespace TaskManager.Web.Extensions
{
    public static class ServiceCollectionExtensions  //Inyeccion de dependecias sobre est metodo par ano tocar el Program.cs
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            // 1. Clientes de API (Capa de Infraestructura)
            //170226 ciomenta  services.AddHttpClient<ITaskApiClient, TaskApiClient>();
            // services.AddHttpClient<ICategoryApiClient, CategoryApiClient>();

            services.AddTransient<ApiExceptionHandler>();

            services.AddHttpClient<ITaskApiClient, TaskApiClient>()
                    .AddHttpMessageHandler<ApiExceptionHandler>();

            services.AddHttpClient<ICategoryApiClient, CategoryApiClient>()
                    .AddHttpMessageHandler<ApiExceptionHandler>();
            // 2. Servicios de Negocio (Service Layer)
            // Se usa AddScoped para que el servicio viva lo que dura la petición HTTP
            services.AddScoped<ITaskService, TaskService>(); //IMplementacióin de Service Layer
            services.AddScoped<ICategoryService, CategoryService>(); //IMplementacióin de Service Layer

            return services;
        }
    }
}
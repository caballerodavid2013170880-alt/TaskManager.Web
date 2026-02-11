using TaskManager.Web.Services;

namespace TaskManager.Web.Extensions
{
    public static class ServiceCollectionExtensions  //Inyeccion de dependecias sobre est metodo par ano tocar el Program.cs
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            services.AddHttpClient<ITaskApiClient, TaskApiClient>();
            services.AddHttpClient<ICategoryApiClient, CategoryApiClient>();


            return services;
        }
    }
}
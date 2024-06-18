using ProApp.Data.Repositories;
using ProApp.Models.IRepositories;

namespace ProApp.Data.IoC
{
    public static class DI
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}

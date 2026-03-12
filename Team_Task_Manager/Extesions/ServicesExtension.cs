using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;

namespace Team_Task_Manager.Extesions
{
    public static class ServicesExtension
    {
        public static IServiceCollection TaskServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<TaskAppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}

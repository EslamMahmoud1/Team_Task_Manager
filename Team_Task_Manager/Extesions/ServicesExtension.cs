using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Services.Implementations;
using Team_Task_Manager.Services.Interfaces;

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

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ITaskService, TaskService>();

            return services;
        }
    }
}

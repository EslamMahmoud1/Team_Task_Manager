using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Models.Entities.Role;
using Team_Task_Manager.Models.Entities.User;
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

            services.AddIdentity<TaskUser, UserRoles>()
                .AddEntityFrameworkStores<TaskAppDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "TaskAppAuthCookie";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7); 
                options.SlidingExpiration = true; 
            });
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            services.AddScoped<IRoleService, RoleService>();
            new MappingConfiguration().ConfigureMappings();
            return services;
        }
    }
}

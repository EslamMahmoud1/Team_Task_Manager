using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Data;
using Team_Task_Manager.Data.Seeding;

namespace Team_Task_Manager.Extesions
{
    public static class AppExtensions
    {
        public async static Task<WebApplication> MigrateAndSeedAsync(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var LoggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskAppDbContext>();
                await context.Database.MigrateAsync();
                await DataSeeding.Seed(context);

            }
            catch (Exception ex)
            {
                var logger = LoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }
            return app;
        }
    }
}

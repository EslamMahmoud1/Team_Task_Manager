using Team_Task_Manager.Extesions;

namespace Team_Task_Manager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.TaskServices(builder.Configuration);
            builder.Services.AddFluentEmail(builder.Configuration);

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=RegisterBasic}/{id?}");
            await app.MigrateAndSeedAsync();
            app.Run();
        }
    }
}

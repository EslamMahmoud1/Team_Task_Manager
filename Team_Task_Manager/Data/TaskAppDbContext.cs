using Microsoft.EntityFrameworkCore;
using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.Models.Entities.User;

namespace Team_Task_Manager.Data
{
    public class TaskAppDbContext : DbContext
    {
        public TaskAppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskAppDbContext).Assembly);
        }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskUser> Users { get; set; }
    }
}

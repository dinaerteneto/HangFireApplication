using Microsoft.EntityFrameworkCore;

namespace HangFireApplication.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Models.TaskScheduler> TaskSchedulers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.TaskScheduler>()
                .ToTable("TaskSchedulers") // Define o nome da tabela
                .HasOne(t => t.DependentTask)
                .WithMany()
                .HasForeignKey(t => t.DependentTaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MiniTaskManager.Models;

namespace MiniTaskManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(p => p.Title).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(t => t.Title).HasMaxLength(150).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(2000);

            // رابطه One-to-Many: با حذف پروژه، وظایف آن هم حذف می‌شوند.
            entity.HasOne(t => t.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => t.ProjectId);
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.DueDate);
        });
    }
}

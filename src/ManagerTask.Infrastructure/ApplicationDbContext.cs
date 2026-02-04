using ManagerTask.Domain.Entities.TableEntity;
using Microsoft.EntityFrameworkCore;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    DbSet<Task> Tasks { get; set; }
    private DbSet<Table> Tables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
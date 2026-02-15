using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Domain.Entities.TableEntity;
using Microsoft.EntityFrameworkCore;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
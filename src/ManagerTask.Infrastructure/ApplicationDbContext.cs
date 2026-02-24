using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Entities.BaseEntity;
using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.UserEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IPublisher _publisher;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }
    
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventAsync();

        return result;
    }

    private async Task PublishDomainEventAsync()
    {
        var events = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();
                return domainEvents;
            }).ToList();

        foreach (var @event in events)
        {
            await _publisher.Publish(@event);
        }
    }
}
using ManagerTask.Domain.Entities.TableEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Application.Abstracts;

public interface IApplicationDbContext
{
    DbSet<Task> Tasks { get; set; }
    DbSet<Table> Tables { get; set; }
    DatabaseFacade Database { get; }    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
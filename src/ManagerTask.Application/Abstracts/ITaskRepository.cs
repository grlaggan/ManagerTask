using FluentResults;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Application.Abstracts;

public interface ITaskRepository
{
    public Task<Result<Guid>> CreateAsync(TaskEntity? task, CancellationToken cancellationToken);
    public Task<Result<TaskEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Result<TaskEntity>> GetByNameAsync(string name, CancellationToken cancellationToken);
}
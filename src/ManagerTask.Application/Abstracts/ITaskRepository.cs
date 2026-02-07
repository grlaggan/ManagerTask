using FluentResults;
using ManagerTask.Domain.Entities.TableEntity;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Application.Abstracts;

public interface ITaskRepository
{
    public Task<Result<Guid>> CreateAsync(TaskEntity? task, CancellationToken cancellationToken);
    public Task<Result<TaskEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Result<TaskEntity>> GetByNameAsync(string name, CancellationToken cancellationToken);
    public Task<Result<List<TaskEntity>>> GetAllAsync(CancellationToken cancellationToken);
    public Task<Result<Guid>> UpdateTaskAsync(Guid TaskId, string Name, string Description, Table Table, DateTime SendTime, CancellationToken cancellationToken);
}
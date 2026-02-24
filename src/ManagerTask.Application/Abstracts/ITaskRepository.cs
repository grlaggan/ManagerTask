using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.UserEntity;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Application.Abstracts;

public interface ITaskRepository
{
    public Task<Result<Guid>> CreateAsync(TaskEntity? task, CancellationToken cancellationToken);
    public Task<Result<TaskEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Result<TaskEntity>> GetByNameAsync(string name, CancellationToken cancellationToken);
    public Task<Result<List<TaskEntity>>> GetAllAsync(PaginationParams @params, User user, CancellationToken cancellationToken);
    public Task<int> GetCountAsync(string? tableName, User user, CancellationToken cancellationToken);
    public Task<Result<Guid>> UpdateTaskAsync(Guid TaskId, string Name, string Description, Table Table, DateTime SendTime, CancellationToken cancellationToken);
    public Task<Result<List<TaskEntity>>> GetWithDeadlineAsync(Deadline deadline, User user, CancellationToken cancellationToken);
    public Task<Result> UpdateStatusAsync(TaskEntity task, CancellationToken cancellationToken);
    public Task<Result> UpdateStatusFailedAsync(TaskEntity task, CancellationToken cancellationToken);
}
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.TableEntity;
using Microsoft.EntityFrameworkCore;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IApplicationDbContext _context;
    private readonly ITableRepository _tableRepository;

    public TaskRepository(IApplicationDbContext context, ITableRepository tableRepository)
    {
        _context = context;
        _tableRepository = tableRepository;
    }

    public async Task<Result<Guid>> CreateAsync(Task? task, CancellationToken cancellationToken)
    {
        if (task is null)
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Task.TaskNull, "Task cannot be null"));

        await _context.Tasks.AddAsync(task, cancellationToken);

        return task.Id;
    }

    public async Task<Result<List<Task>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tasks = await _context.Tasks.Include(t => t.Table).ToListAsync(cancellationToken);

        return tasks;
    }

    public async Task<Result<Task>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (task is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Task.TaskNotFound, "Task not found"));

        return task;
    }

    public async Task<Result<Task>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);

        if (task is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Task.TaskNotFound, "Task not found"));

        return task;
    }

    public async Task<Result<Guid>> UpdateTaskAsync(Guid TaskId, string Name, string Description, Table table, DateTime SendTime, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == TaskId, cancellationToken);

        if (task is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Task.TaskNotFound, "Task not found"));

        task.Name = Name;
        task.Description = Description;
        task.SendTime = SendTime;
        task.Table = table;

        return task.Id;
    }
}
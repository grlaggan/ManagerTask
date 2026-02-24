using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Common;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.TaskEntity;
using ManagerTask.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;
using static ManagerTask.Infrastructure.Extensions.TasksExtension;
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

        var checkTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

        await _context.Tasks.AddAsync(task, cancellationToken);

        return task.Id;
    }

    public async Task<Result<List<Task>>> GetAllAsync(PaginationParams @params, User user, CancellationToken cancellationToken)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Table)
            .Where(t => t.User.ChatId == user.ChatId)
            .Page(@params)
            .ToListAsync(cancellationToken);

        return tasks;
    }

    public async Task<int> GetCountAsync(string? tableName, User user, CancellationToken cancellationToken)
    {
        var count = tableName is null ? await _context.Tasks
                .Include(t => t.User)
                .Where(t => t.User.ChatId == user.ChatId).CountAsync(cancellationToken) : 
            await _context.Tasks.Where(t => t.Table.Name == tableName).CountAsync(cancellationToken);

        return count;
    }

    public async Task<Result<Task>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.Include(t => t.Table)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (task is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Task.TaskNotFound, "Task not found"));

        return task;
    }

    public async Task<Result<Task>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.Include(t => t.Table)
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);

        if (task is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Task.TaskNotFound, "Task not found"));

        return task;
    } 

    public async Task<Result<List<Task>>> GetWithDeadlineAsync(Deadline deadline, User user, CancellationToken cancellationToken)
    {

        if (deadline.Minutes != 0)
        {
            var tasks1 = await _context
                .Tasks
                .Include(t => t.User)
                .Where(t => t.User.ChatId == user.ChatId)
                .Include(t => t.Table).Where(
                t => (t.SendTime - DateTime.UtcNow).TotalMinutes < deadline.Minutes && (t.SendTime - DateTime.UtcNow).TotalMinutes > 0).ToListAsync(cancellationToken);
            return tasks1;
        }

        if (deadline.Hours != 0)
        {
            var tasks2 = await _context
                .Tasks
                .Include(t => t.User)
                .Where(t => t.User.ChatId == user.ChatId)
                .Include(t => t.Table).Where(
                t => (t.SendTime - DateTime.UtcNow).TotalHours < deadline.Hours && (t.SendTime - DateTime.UtcNow).TotalMinutes > 0).ToListAsync(cancellationToken);
            return tasks2;
        }

        var tasks = await _context
            .Tasks
            .Include(t => t.User)
            .Where(t => t.User.ChatId == user.ChatId)
            .Include(t => t.Table).Where(
                t => (t.SendTime - DateTime.UtcNow).TotalDays < deadline.Days && (t.SendTime - DateTime.UtcNow).TotalMinutes > 0).ToListAsync(cancellationToken);
        return tasks;
    }

    public async Task<Result> UpdateStatusAsync(Task task, CancellationToken cancellationToken)
    {
        task.Status = StatusTask.Completed;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> UpdateStatusFailedAsync(Task task, CancellationToken cancellationToken)
    {
        task.Status = StatusTask.Failed;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
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
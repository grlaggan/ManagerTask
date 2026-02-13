using FluentResults;
using ManagerTask.Application.Abstracts;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;
using ManagerTask.Application.Commands.Task;
using MediatR;
using ManagerTask.Domain.Common.Errors;
using Quartz;
using ManagerTask.Infrastructure.Jobs;

namespace ManagerTask.Application.Handlers.Task;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ITableRepository _tableRepository;
    private readonly ISchedulerFactory _schedulerFactory;

    public CreateTaskHandler(ITaskRepository repository, ITransactionManager transactionManager,
        ITableRepository tableRepository, ISchedulerFactory schedulerFactory)
    {
        _taskRepository = repository;
        _transactionManager = transactionManager;
        _tableRepository = tableRepository;
        _schedulerFactory = schedulerFactory;
    }

    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var transactionBeginResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        if (transactionBeginResult.IsFailed)
            return Result.Fail(transactionBeginResult.Errors[0]);

        using var transactionScope = transactionBeginResult.Value;

        var resultGetByName = await _taskRepository.GetByNameAsync(request.Name, cancellationToken);

        if (resultGetByName.IsSuccess)
        {
            transactionScope.Rollback();
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Task.TaskAlreadyExists, "Task with the same name already exists"));
        }

        var tableResult = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (tableResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(tableResult.Errors[0]);
        }

        var taskResult = TaskEntity.Create(request.Name, request.Description, request.SendTime, tableResult.Value);

        if (taskResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(taskResult.Errors[0]);
        }


        var result = await _taskRepository.CreateAsync(taskResult.Value, cancellationToken);

        if (result.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(result.Errors[0]);
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(saveChangesResult.Errors[0]);
        }

        var resultCommit = transactionScope.Commit();

        if (resultCommit.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultCommit.Errors[0]);
        }

        JobBuilder.Create<TaskNotificationJob>()
            .WithIdentity($"TaskNotification-{taskResult.Value.Id}")
            .UsingJobData("Name", taskResult.Value.Name)
            .UsingJobData("Description", taskResult.Value.Description)
            .UsingJobData("TableName", tableResult.Value.Name)
            .UsingJobData("Minutes", tableResult.Value.Name)

        return result.Value;
    }

    private async Task CreateJob(TaskEntity task, string tableName, IScheduler scheduler)
    {
        var job = JobBuilder.Create<TaskNotificationJob>()
                    .WithIdentity($"TaskNotificatin-{task.Id}")
                    .UsingJobData("Name", task.Name)
                    .UsingJobData("Description", task.Description)
                    .UsingJobData("TableName", tableName)
                    .UsingJobData("Minutes", 5)
                    .UsingJobData("Hours", 0)
                    .UsingJobData("Days", 0).Build();

        var trigger = TriggerBuilder.Create()
                    .WithIdentity($"TaskNotificationTrigger-{task.Id}")
                    .StartAt(DateTimeOffset.Parse(task.SendTime.ToString()))
                    .Build();


    }
}
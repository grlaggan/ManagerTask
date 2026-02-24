using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;
using ManagerTask.Application.Commands.Task;
using ManagerTask.Application.Jobs;
using MediatR;
using ManagerTask.Domain.Common.Errors;
using Quartz;
using ManagerTask.Application.Jobs;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ITableRepository _tableRepository;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IUserRepository _userRepository;

    public CreateTaskHandler(ITaskRepository repository, ITransactionManager transactionManager,
        ITableRepository tableRepository, ISchedulerFactory schedulerFactory, IUserRepository userRepository)
    {
        _taskRepository = repository;
        _transactionManager = transactionManager;
        _tableRepository = tableRepository;
        _schedulerFactory = schedulerFactory;
        _userRepository = userRepository;
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

        var resultGetUserByChatId = await _userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultGetUserByChatId.Errors[0]);
        }

        var user = resultGetUserByChatId.Value;

        var tableResult = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (tableResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(tableResult.Errors[0]);
        }

        var taskResult = TaskEntity.Create(request.Name, request.Description, request.SendTime, user, tableResult.Value);

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
        
        return result.Value;
    }
}
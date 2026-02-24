using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Task;
using ManagerTask.Application.Jobs;
using ManagerTask.Domain.Common.Errors;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;
using MediatR;
using Quartz;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class CreateTaskByTableNameHandler(
    ITaskRepository taskRepository,
    ITableRepository tableRepository,
    ITransactionManager transactionManager,
    ISchedulerFactory schedulerFactory,
    IUserRepository userRepository) : IRequestHandler<CreateTaskByTableNameCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateTaskByTableNameCommand request, CancellationToken cancellationToken)
    {
        var beginTransactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

        if (beginTransactionResult.IsFailed)
            return Result.Fail(beginTransactionResult.Errors[0]);

        using var transactionScope = beginTransactionResult.Value;

        var resultGetByName = await taskRepository.GetByNameAsync(request.Name, cancellationToken);

        if (resultGetByName.IsSuccess)
        {
            transactionScope.Rollback();
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Task.TaskAlreadyExists, "Task with the same name already exists"));
        }

        var resultGetUserByChatId = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultGetUserByChatId.Errors[0]);
        }

        var user = resultGetUserByChatId.Value;
        
        var resultGetTableByTableName = await tableRepository.GetByNameAsync(request.TableName, cancellationToken);

        if (resultGetTableByTableName.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultGetTableByTableName.Errors[0]);
        }

        var table = resultGetTableByTableName.Value;

        var resultCreationTask = TaskEntity.Create(request.Name, request.Description, request.SendTime, user, table);

        if (resultCreationTask.IsFailed)
            return Result.Fail(resultCreationTask.Errors[0]);

        var result = await taskRepository.CreateAsync(resultCreationTask.Value, cancellationToken);

        if (result.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(result.Errors[0]);
        }

        var resultSaveChanges = await transactionManager.SaveChangesAsync(cancellationToken);

        if (resultSaveChanges.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultSaveChanges.Errors[0]);
        }

        var resultCommit = transactionScope.Commit();

        if (resultCommit.IsFailed)
            return Result.Fail(resultCommit.Errors[0]);
        
        return result.Value;
    }
}
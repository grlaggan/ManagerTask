using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Task;
using MediatR;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class UpdateTaskHandler(
    ITaskRepository taskRepository,
    ITableRepository tableRepository,
    ITransactionManager transactionManager) : IRequestHandler<UpdateTaskCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var resultBeginTransaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        if (resultBeginTransaction.IsFailed)
            return Result.Fail(resultBeginTransaction.Errors);

        using var transactionScope = resultBeginTransaction.Value;

        var tableResult = await tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (tableResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(tableResult.Errors[0]);
        }

        var updateResult = await taskRepository.UpdateTaskAsync(request.TaskId, request.Name, request.Description, tableResult.Value, request.SendTime, cancellationToken);

        if (updateResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(updateResult.Errors[0]);
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

        return updateResult;
    }
}
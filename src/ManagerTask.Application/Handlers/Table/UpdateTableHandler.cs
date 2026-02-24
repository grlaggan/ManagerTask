using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Table;
using MediatR;

namespace ManagerTask.Application.Handlers.Table;

public class UpdateTableHandler(
    ITableRepository tableRepository,
    ITransactionManager transactionManager) : IRequestHandler<UpdateTableCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var resultBeginTransaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        if (resultBeginTransaction.IsFailed)
            return Result.Fail(resultBeginTransaction.Errors[0]);

        using var transactionScope = resultBeginTransaction.Value;

        var resultUpdateTable = await tableRepository.UpdateTableAsync(request.TableId, request.Name, request.Description, cancellationToken);

        if (resultUpdateTable.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultUpdateTable.Errors[0]);
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

        return request.TableId;
    }

}

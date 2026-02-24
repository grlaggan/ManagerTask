using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Common.Errors;
using Microsoft.EntityFrameworkCore.Storage;

namespace ManagerTask.Infrastructure.Common;

public class TransactionManager(IApplicationDbContext context) : ITransactionManager
{
    public async Task<Result<ITransactionScope>> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            var transactionScope = new TransactionScope(transaction.GetDbTransaction());
            return transactionScope;
        }
        catch
        {
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Transaction.BeginFailed, "Could not begin transaction"));
        }
    }

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch
        {
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Transaction.CommitFailed, "Could not save changes"));
        }
    }
}
using System.Data;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Common.Errors;

namespace ManagerTask.Infrastructure.Common;

public class TransactionScope(IDbTransaction transaction) : ITransactionScope
{

    public Result Commit()
    {
        try
        {
            transaction.Commit();
            return Result.Ok();
        }
        catch
        {
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Transaction.CommitFailed, "Transaction commit failed"));
        }
    }

    public Result Rollback()
    {
        try
        {
            transaction.Rollback();
            return Result.Ok();
        }
        catch
        {
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Transaction.RollbackFailed, "Transaction rollback failed"));
        }
    }

    public void Dispose()
    {
        transaction.Dispose();
    }
}
using System.Data;
using FluentResults;
using ManagerTask.Application.Abstracts;

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
            return Result.Fail("Transaction commit failed");
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
            return Result.Fail("Transaction rollback failed");
        }
    }

    public void Dispose()
    {
        transaction.Dispose();
    }
}
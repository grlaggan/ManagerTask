using System;
using FluentResults;

namespace ManagerTask.Application.Abstracts;

public interface ITransactionScope : IDisposable
{
    public Result Commit();
    public Result Rollback();
}
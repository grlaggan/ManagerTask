using FluentResults;

namespace ManagerTask.Application.Abstracts;

public interface ITransactionManager
{
    Task<Result<ITransactionScope>> BeginTransactionAsync(CancellationToken cancellationToken);
    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken);
}
using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace ManagerTask.Application.Abstracts;

public interface ITransactionManager
{
    public Task<Result<ITransactionScope>> BeginTransactionAsync(CancellationToken cancellationToken);
    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken);
}
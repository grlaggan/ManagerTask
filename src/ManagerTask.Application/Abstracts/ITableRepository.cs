using FluentResults;
using ManagerTask.Domain.Entities.TableEntity;

namespace ManagerTask.Application.Abstracts;

public interface ITableRepository
{
    public Task<Result<Table>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Result<Guid>> CreateAsync(Table? table, CancellationToken cancellationToken);
    public Task<Result<Table>> GetByNameAsync(string name, CancellationToken cancellationToken);
}

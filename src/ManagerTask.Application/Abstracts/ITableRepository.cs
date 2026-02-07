using FluentResults;
using ManagerTask.Domain.Entities.TableEntity;

namespace ManagerTask.Application.Abstracts;

public interface ITableRepository
{
    public Task<Result<Table>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Result<Guid>> CreateAsync(Table? table, CancellationToken cancellationToken);
    public Task<Result<Table>> GetByNameAsync(string name, CancellationToken cancellationToken);
    public Task<Result<List<Table>>> GetAllAsync(CancellationToken cancellationToken);
    public Task<Result> SaveAsync(CancellationToken cancellationToken);
    public Task<Result<Guid>> UpdateTableAsync(Guid TableId, string Name, string Description, CancellationToken cancellationToken);
}

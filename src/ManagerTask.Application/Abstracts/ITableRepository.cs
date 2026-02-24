using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Application.Abstracts;

public interface ITableRepository
{
    public Task<Result<Table>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Result<Guid>> CreateAsync(Table? table, CancellationToken cancellationToken);
    public Task<Result<Table>> GetByNameAsync(string name, CancellationToken cancellationToken);
    public Task<Result<List<Table>>> GetAllAsync(PaginationParams @params, User user, CancellationToken cancellationToken);
    public Task<int> GetCountAsync(CancellationToken cancellationToken);
    public Task<Result> SaveAsync(CancellationToken cancellationToken);
    public Task<Result<Guid>> UpdateTableAsync(Guid TableId, string Name, string Description, CancellationToken cancellationToken);
}

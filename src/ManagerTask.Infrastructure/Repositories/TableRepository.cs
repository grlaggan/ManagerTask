using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.TableEntity;
using Microsoft.EntityFrameworkCore;

namespace ManagerTask.Infrastructure.Repositories;

public class TableRepository(IApplicationDbContext context) : ITableRepository
{
    public async Task<Result<Guid>> CreateAsync(Table? table, CancellationToken cancellationToken)
    {
        if (table is null)
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Table.TableNull, "Table cannot be null"));

        await context.Tables.AddAsync(table, cancellationToken);

        return table.Id;
    }

    public async Task<Result<List<Table>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tables = await context.Tables.ToListAsync(cancellationToken);
        return tables;
    }

    public async Task<Result<Table>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (table is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Table.TableNotFound, "Table not found"));

        return table;
    }

    public async Task<Result<Table>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var result = await context.Tables.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);

        if (result is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Table.TableNotFound, "Table not found"));

        return result;
    }

    public async Task<Result> SaveAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }

}

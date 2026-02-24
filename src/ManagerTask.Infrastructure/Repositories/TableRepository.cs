using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Common;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.UserEntity;
using ManagerTask.Infrastructure.Extensions;
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

    public async Task<Result<List<Table>>> GetAllAsync(PaginationParams @params, User user, CancellationToken cancellationToken)
    {
        var tables = await context.Tables
            .Include(t => t.User)
            .Where(t => t.User.ChatId == user.ChatId)
            .Page(@params)
            .ToListAsync(cancellationToken);
        return tables;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        var count = await context.Tables.CountAsync(cancellationToken);

        return count;
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

    public async Task<Result<Guid>> UpdateTableAsync(Guid TableId, string Name, string Description, CancellationToken cancellationToken)
    {
        var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == TableId, cancellationToken);

        if (table is null)
            return Result.Fail(ApplicationError.NotFound(ErrorCodes.Table.TableNotFound, "Table not found"));

        table.Name = Name;
        table.Description = Description;

        return table.Id;
    }
}

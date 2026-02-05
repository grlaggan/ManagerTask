using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Table;
using TableEntity = ManagerTask.Domain.Entities.TableEntity.Table;
using MediatR;

namespace ManagerTask.Application.Handlers.Table;

public class CreateTableHandler(ITableRepository repository) : IRequestHandler<CreateTableCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var getTableByNameResult = await repository.GetByNameAsync(request.Name, cancellationToken);

        if (getTableByNameResult.IsSuccess)
            return Result.Fail("Table with the same name already exists.");

        var tableResult = TableEntity.Create(request.Name, request.Description);

        if (tableResult.IsFailed)
            return Result.Fail(tableResult.Errors[0]);

        var createResult = await repository.CreateAsync(tableResult.Value, cancellationToken);

        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors[0]);

        return tableResult.Value.Id;
    }

}

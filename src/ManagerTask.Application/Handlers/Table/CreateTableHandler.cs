using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Table;
using TableEntity = ManagerTask.Domain.Entities.TableEntity.Table;
using MediatR;
using ManagerTask.Domain.Common.Errors;

namespace ManagerTask.Application.Handlers.Table;

public class CreateTableHandler(ITableRepository repository) : IRequestHandler<CreateTableCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var getTableByNameResult = await repository.GetByNameAsync(request.Name, cancellationToken);

        if (getTableByNameResult.IsSuccess)
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Table.TableAlreadyExists, "Table with the same name already exists"));

        var tableResult = TableEntity.Create(request.Name, request.Description);

        if (tableResult.IsFailed)
            return Result.Fail(tableResult.Errors[0]);

        var createResult = await repository.CreateAsync(tableResult.Value, cancellationToken);

        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors[0]);

        var saveResult = await repository.SaveAsync(cancellationToken);

        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors[0]);

        return tableResult.Value.Id;
    }

}

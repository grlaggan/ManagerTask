using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using ManagerTask.Domain.Common.Errors;
using MediatR;

namespace ManagerTask.Application.Handlers.Table;

public class GetTablesHandler(ITableRepository repository, IMapper mapper) : IRequestHandler<GetTablesQuery, Result<List<TableDto>>>
{

    public async Task<Result<List<TableDto>>> Handle(GetTablesQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync(cancellationToken);
        if (result.IsFailed)
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Table.TableFailedRetrieveTables, "Failed to retrieve tables"));

        return result.Value.Select(t => mapper.Map<TableDto>(t)).ToList();
    }

}

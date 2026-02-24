using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using ManagerTask.Domain.Common.Errors;
using MediatR;

namespace ManagerTask.Application.Handlers.Table;

public class GetTablesHandler(
    ITableRepository repository,
    IMapper mapper,
    IUserRepository userRepository) : IRequestHandler<GetTablesQuery, Result<GetTablesResultHandle>>
{

    public async Task<Result<GetTablesResultHandle>> Handle(GetTablesQuery request, CancellationToken cancellationToken)
    {
        var resultGetUserByChatId = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
            return Result.Fail(resultGetUserByChatId.Errors[0]);

        var user = resultGetUserByChatId.Value;

        var result = await repository.GetAllAsync(request.Params, user, cancellationToken);
        var tasks = result.Value;
        
        if (result.IsFailed)
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Table.TableFailedRetrieveTables, "Failed to retrieve tables"));

        var offset = request.Params.Offset ?? 3;
        var countPages = (int) Math.Ceiling(Convert.ToDouble(await repository.GetCountAsync(cancellationToken)) / offset);

        var resultHandle = new GetTablesResultHandle(
            tasks.Select(t => mapper.Map<TableDto>(t)).ToList(),
            countPages
        );

        return resultHandle;
    }

}

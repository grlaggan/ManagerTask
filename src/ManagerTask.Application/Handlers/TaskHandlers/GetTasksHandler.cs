using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Common;
using ManagerTask.Application.Models.Dtos;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;
using ManagerTask.Application.Queries;
using ManagerTask.Domain.Common.Errors;
using MediatR;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class GetTasksHandler(IMapper mapper,
    ITaskRepository repository,
    IUserRepository userRepository) : IRequestHandler<GetTasksQuery,
    Result<GetTasksResultHandle>>
{

    public async Task<Result<GetTasksResultHandle>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var resultGetUserByChatId = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
            return Result.Fail(resultGetUserByChatId.Errors[0]);

        var user = resultGetUserByChatId.Value;

        var result = await repository.GetAllAsync(request.PaginationParams, user, cancellationToken);

        var tasks = result.Value;
        if (request.TableName is not null)
            tasks = tasks.Where(t => t.Table.Name == request.TableName).ToList();
                
        if (result.IsFailed)
            return Result.Fail(ApplicationError
                .Conflict(ErrorCodes.Task.TaskFailedRetrieveTasks, "Failed to retrieve tasks"));

        var offset = request.PaginationParams.Offset ?? 3;
        
        var countPages = (int) Math.Ceiling(Convert.ToDouble(await repository
            .GetCountAsync(request.TableName, user, cancellationToken)) / offset);

        var resultHandle = new GetTasksResultHandle(
            tasks.Select(task => mapper.Map<TaskDto>(task)).ToList(),
            countPages);

        return resultHandle;
    }

}
using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Models.Profiles;
using ManagerTask.Domain.Common.Errors;
using MediatR;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class GetTasksHandler(IMapper mapper, ITaskRepository repository) : IRequestHandler<GetTasksQuery, Result<List<TaskDto>>>
{

    public async Task<Result<List<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync(cancellationToken);

        if (result.IsFailed)
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Task.TaskFailedRetrieveTasks, "Failed to retrieve tasks"));

        return result.Value.Select(task => mapper.Map<TaskDto>(task)).ToList();
    }

}
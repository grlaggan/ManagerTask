using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using MediatR;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class GetTaskHandler(ITaskRepository taskRepository, IMapper mapper) : IRequestHandler<GetTaskQuery, Result<TaskDto>>
{

    public async Task<Result<TaskDto>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var result = await taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors[0]);
        
        return mapper.Map<TaskDto>(result.Value);
    }
}
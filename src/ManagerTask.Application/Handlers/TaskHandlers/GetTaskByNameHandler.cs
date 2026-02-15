using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using MediatR;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class GetTaskByNameHandler(
    ITaskRepository taskRepository,
    IMapper mapper) : IRequestHandler<GetTaskByNameQuery, Result<TaskDto>>
{

    public async Task<Result<TaskDto>> Handle(GetTaskByNameQuery request, CancellationToken cancellationToken)
    {
        var result = await taskRepository.GetByNameAsync(request.Name, cancellationToken);
        
        if (result.IsFailed)
            return Result.Fail(result.Errors[0]);

        var task = result.Value;

        var dto = mapper.Map<TaskDto>(task);

        return dto;
    }
}
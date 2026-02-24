using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Task;
using MediatR;

namespace ManagerTask.Application.Handlers.TaskHandlers;

public class UpdateStatusFailedTaskHandler(ITaskRepository taskRepository)
    : IRequestHandler<UpdateStatusFailedTaskCommand, Result>
{

    public async Task<Result> Handle(UpdateStatusFailedTaskCommand request, CancellationToken cancellationToken)
    {
        var resultGetTask = await taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (resultGetTask.IsFailed)
            return Result.Fail(resultGetTask.Errors[0]);

        var resultUpdateStatus = await taskRepository.UpdateStatusFailedAsync(resultGetTask.Value, cancellationToken);
        
        if (resultUpdateStatus.IsFailed)
            return Result.Fail(resultUpdateStatus.Errors[0]);
        
        return Result.Ok();
    }
}
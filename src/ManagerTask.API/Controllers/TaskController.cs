using ManagerTask.Application.Commands.Task;
using ManagerTask.Application.Models.Profiles;
using ManagerTask.Dtos.Task;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask;

[ApiController]
[Route("api/tasks")]
public class TaskController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateTaskResponse>> CreateTaskAsync([FromBody] CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(request.Name, request.Description, request.TableId, request.SendTime);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return result.ToProblemDetails<CreateTaskResponse>(HttpContext);

        var response = new CreateTaskResponse("Task created successfully", result.Value);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetTasksResponse>> GetTasksAsync()
    {
        var query = new GetTasksQuery();
        var result = await mediator.Send(query);

        if (result.IsFailed)
            return result.ToProblemDetails<GetTasksResponse>(HttpContext);

        var response = new GetTasksResponse("Tasks retrieved successfully", result.Value);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateTaskResponse>> UpdateTaskAsync(
        [FromBody] UpdateTaskRequest request, [FromRoute] Guid id)
    {
        var command = new UpdateTaskCommand(id, request.Name, request.Description, request.TableId, request.SendTime);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return result.ToProblemDetails<UpdateTaskResponse>(HttpContext);

        var response = new UpdateTaskResponse("Task updated successfully", result.Value);
        return Ok(response);
    }
}

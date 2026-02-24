using ManagerTask.Application.Commands.Task;
using ManagerTask.Application.Common;
using ManagerTask.Application.Handlers.TaskHandlers;
using ManagerTask.Application.Models.Profiles;
using ManagerTask.Application.Queries;
using ManagerTask.Dtos.Task;
using ManagerTask.Models.Dtos.Task;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateTaskResponse>> CreateTaskAsync(
        [FromBody] CreateTaskRequest request,
        [FromHeader] string chatId)
    {
        var command = new CreateTaskCommand(request.Name, request.Description, chatId, request.TableId, request.SendTime);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return result.ToProblemDetails<CreateTaskResponse>(HttpContext);

        var response = new CreateTaskResponse("Task created successfully", result.Value);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetTaskResponse>> GetTaskByIdAsync([FromRoute] Guid id)
    {
        var result = await mediator.Send(new GetTaskQuery(id));
        
        if (result.IsFailed)
            return result.ToProblemDetails<GetTaskResponse>(HttpContext);

        return Ok(result.Value);
    }
    
    [HttpGet]
    public async Task<ActionResult<GetTasksResponse>> GetTasksAsync([FromQuery] string? tableName, 
        [FromQuery] PaginationParams @params,
        [FromHeader] string chatId)
    {
        var query = new GetTasksQuery(tableName, chatId, @params);
        var result = await mediator.Send(query);

        if (result.IsFailed)
            return result.ToProblemDetails<GetTasksResponse>(HttpContext);

        var page = @params.Page ?? 1;
        var offset = @params.Offset ?? 3;
        var countPages = result.Value.CountPages;
        var response = new GetTasksResponse("Tasks retrieved successfully",
            result.Value.Tasks, page, offset, countPages);

        return Ok(response);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<GetTaskResponse>> GetTaskByName([FromRoute] string name)
    {
        var result = await mediator.Send(new GetTaskByNameQuery(name));
        
        if (result.IsFailed)
            return result.ToProblemDetails<GetTaskResponse>(HttpContext);
        
        return Ok(
            new GetTaskResponse(
                result.Value.Id,
                result.Value.Name,
                result.Value.Description,
                result.Value.Table.Name,
                result.Value.Status));
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

    [HttpPost("byTableName")]
    public async Task<ActionResult<CreateTaskResponse>> CreateTaskByTableNameAsync(
        [FromBody] CreateTaskByTableNameRequest request,
        [FromHeader] string chatId)
    {
        var result = await mediator.Send(new CreateTaskByTableNameCommand(
            request.Name,
            request.Description,
            request.TableName,
            request.SendTime,
            chatId
        ));

        if (result.IsFailed)
            return result.ToProblemDetails<CreateTaskResponse>(HttpContext);

        var response = new CreateTaskResponse("Create task by table name successfully", result.Value);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<PatchTaskStatusResponse>> UpdateTaskStatus([FromRoute] Guid id)
    {
        var result = await mediator.Send(new UpdateStatusTaskCommand(id));

        if (result.IsFailed)
            return result.ToProblemDetails<PatchTaskStatusResponse>(HttpContext);

        return Ok();
    }

    [HttpPatch("{id:guid}/status/failed")]
    public async Task<ActionResult<PatchTaskStatusResponse>> UpdateTaskStatusFailed([FromRoute] Guid id)
    {
        var result = await mediator.Send(new UpdateStatusFailedTaskCommand(id));

        if (result.IsFailed)
            return result.ToProblemDetails<PatchTaskStatusResponse>(HttpContext);
        
        return Ok();
    }
}

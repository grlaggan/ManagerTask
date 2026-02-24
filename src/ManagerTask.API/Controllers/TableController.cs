using ManagerTask.Application.Commands.Table;
using ManagerTask.Application.Common;
using ManagerTask.Application.Handlers.Table;
using ManagerTask.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask.Controllers;

[ApiController]
[Route("/api/tables")]
public class TableController(IMediator mediator) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<CreateTableResponse>> CreateTableAsync(
        [FromBody] CreateTableRequest request,
        [FromHeader] string chatId)
    {
        var command = new CreateTableCommand(request.Name, request.Description, chatId);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return result.ToProblemDetails<CreateTableResponse>(HttpContext);

        var response = new CreateTableResponse("Table created successfully", result.Value);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetTablesResponse>> GetTablesAsync(
        [FromQuery] PaginationParams @params,
        [FromHeader] string chatId)
    {
        var query = new GetTablesQuery(@params, chatId);
        var result = await mediator.Send(query);

        if (result.IsFailed)
            return result.ToProblemDetails<GetTablesResponse>(HttpContext);

        var resultHandle = result.Value;

        var page = @params.Page ?? 1;
        var offset = @params.Offset ?? 3;
        var countPages = resultHandle.CountPages;

        var response = new GetTablesResponse(resultHandle.Tables, page, offset, countPages);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateTableResponse>> UpdateTableAsync(
        [FromBody] UpdateTableRequest request, [FromRoute] Guid id)
    {
        var command = new UpdateTableCommand(id, request.Name, request.Description);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return result.ToProblemDetails<UpdateTableResponse>(HttpContext);

        var response = new UpdateTableResponse("Table updated successfully", result.Value);
        return Ok(response);
    }
}
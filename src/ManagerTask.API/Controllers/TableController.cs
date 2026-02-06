using ManagerTask.Application.Commands.Table;
using ManagerTask.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask.Controllers;

[ApiController]
[Route("/api/tables")]
public class TableController(IMediator mediator) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<CreateTableResponse>> CreateTableAsync([FromBody] CreateTableRequest request)
    {
        var command = new CreateTableCommand(request.Name, request.Description);
        var result = await mediator.Send(command);

        if (result.IsFailed)
            return result.ToProblemDetails<CreateTableResponse>(HttpContext);

        var response = new CreateTableResponse("Table created successfully", result.Value);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetTablesResponse>> GetTablesAsync()
    {
        var query = new GetTablesQuery();
        var result = await mediator.Send(query);

        if (result.IsFailed)
            return result.ToProblemDetails<GetTablesResponse>(HttpContext);

        var response = new GetTablesResponse(result.Value);
        return Ok(response);
    }
}
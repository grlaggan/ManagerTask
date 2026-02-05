using FluentResults;
using ManagerTask.Application.Commands.Table;
using ManagerTask.Application.Commands.Task;
using ManagerTask.Dtos.Task;
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
            return Problem(
                title: "Error creating table",
                detail: result.Errors[0].Message,
                statusCode: StatusCodes.Status400BadRequest
            );

        var response = new CreateTableResponse("Table created successfully", result.Value);
        return Ok(response);
    }
}
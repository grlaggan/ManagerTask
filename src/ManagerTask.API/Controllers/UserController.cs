using ManagerTask.Application.Commands.User;
using ManagerTask.Application.Queries;
using ManagerTask.Dtos.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> CreateUserAsync([FromBody] CreateUserRequest request)
    {
        var result = await mediator.Send(new CreateUserCommand(request.ChatId));

        if (result.IsFailed)
            return result.ToProblemDetails<CreateUserResponse>(HttpContext);

        var response = new CreateUserResponse("User created successfully", result.Value);
        return Ok(response);
    }

    [HttpGet("{chatId}")]
    public async Task<ActionResult<GetUserResponse>> GetUserByChatIdAsync([FromRoute] string chatId)
    {
        var result = await mediator.Send(new GetUserByChatIdQuery(chatId));
        
        if (result.IsFailed)
            return result.ToProblemDetails<GetUserResponse>(HttpContext);

        return Ok(result.Value);
    }
}
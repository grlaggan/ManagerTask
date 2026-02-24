using ManagerTask.Application.Commands.Notification;
using ManagerTask.Application.Common;
using ManagerTask.Application.Queries;
using ManagerTask.Dtos.Notification;
using ManagerTask.Models.Dtos.Notification;
using ManagerTask.Models.Dtos.Task;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask.Controllers;

[ApiController]
[Route("/api/notifications")]
public class NotificationController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateNotificationResponse>> CreateAsync(
        [FromBody] CreateNotificationRequest request,
        [FromHeader] string chatId)
    {
        var result = await mediator.Send(new CreateNotificationCommand(
            request.Name,
            request.Message,
            request.NotificationTime,
            chatId
        ));

        if (result.IsFailed)
            return result.ToProblemDetails<CreateNotificationResponse>(HttpContext);

        var response = new CreateNotificationResponse("Notification created successfully", result.Value);

        return response;
    }

    [HttpGet]
    public async Task<ActionResult<GetNotificationsResponse>> GetAllAsync(
        [FromQuery] PaginationParams @params,
        [FromHeader] string chatId)
    {
        var result = await mediator.Send(new GetNotificationsQuery(@params, chatId));

        if (result.IsFailed)
            return result.ToProblemDetails<GetNotificationsResponse>(HttpContext);
        
        var page = @params.Page ?? 1;
        var offset = @params.Offset ?? 3;
        var countPages = result.Value.CountPages;

        var response = new GetNotificationsResponse(
            result.Value.Notifications,
            page,
            offset,
            countPages
        );

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetNotificationByIdResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await mediator.Send(new GetNotificationByIdQuery(id));

        if (result.IsFailed)
            return result.ToProblemDetails<GetNotificationByIdResponse>(HttpContext);

        var response = new GetNotificationByIdResponse(
            result.Value.Name,
            result.Value.Message,
            result.Value.NotificationTime,
            result.Value.CreatedAt
        );

        return Ok(response);
    }
}
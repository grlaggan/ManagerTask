using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using MediatR;

namespace ManagerTask.Application.Handlers.Notification;

public class GetNotificationByIdHandler(INotificationRepository notificationRepository, IMapper mapper)
    : IRequestHandler<GetNotificationByIdQuery, Result<NotificationDto>>
{
    public async Task<Result<NotificationDto>> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors[0]);
        
        return mapper.Map<NotificationDto>(result.Value);
    }
}
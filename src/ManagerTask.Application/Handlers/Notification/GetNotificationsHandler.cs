using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using MediatR;

namespace ManagerTask.Application.Handlers.Notification;

public class GetNotificationsHandler(INotificationRepository notificationRepository, IMapper mapper)
    : IRequestHandler<GetNotificationsQuery, Result<GetNotificationsResultHandle>>
{

    public async Task<Result<GetNotificationsResultHandle>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await notificationRepository.GetAllAsync(request.Params, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors[0]);

        var count = await notificationRepository.GetCountAsync(cancellationToken);

        var resultHandle = new GetNotificationsResultHandle(
            result.Value.Select(n => mapper.Map<NotificationDto>(n)).ToList(),
            (int) Math.Ceiling(Convert.ToDouble(count) / request.Params.Offset ?? 3)
        );

        return resultHandle;
    }
}

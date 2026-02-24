using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using MediatR;

namespace ManagerTask.Application.Handlers.Notification;

public class GetNotificationsHandler(
    INotificationRepository notificationRepository,
    IMapper mapper,
    IUserRepository userRepository)
    : IRequestHandler<GetNotificationsQuery, Result<GetNotificationsResultHandle>>
{

    public async Task<Result<GetNotificationsResultHandle>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var resultGetUserByChatId = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
            return Result.Fail(resultGetUserByChatId.Errors[0]);

        var user = resultGetUserByChatId.Value;

        var result = await notificationRepository.GetAllAsync(request.Params, user, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors[0]);

        var count = await notificationRepository.GetCountAsync(user, cancellationToken);

        var resultHandle = new GetNotificationsResultHandle(
            result.Value.Select(n => mapper.Map<NotificationDto>(n)).ToList(),
            (int) Math.Ceiling(Convert.ToDouble(count) / request.Params.Offset ?? 3)
        );

        return resultHandle;
    }
}

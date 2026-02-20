using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Notification;
using ManagerTask.Application.Jobs;
using MediatR;
using Quartz;
using NotificationEntity = ManagerTask.Domain.Entities.NotificationEntity.Notification;

namespace ManagerTask.Application.Handlers.Notification;

public class CreateNotificationHandler(
    INotificationRepository notificationRepository,
    ISchedulerFactory schedulerFactory)
    : IRequestHandler<CreateNotificationCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var resultCreationNotification = NotificationEntity.Create(request.Name, request.Message, request.NotificationTime);
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

        if (resultCreationNotification.IsFailed)
            return Result.Fail(resultCreationNotification.Errors[0]);

        var notification = resultCreationNotification.Value;
        var result = await notificationRepository.CreateAsync(notification, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors[0]);

        Console.WriteLine("in handler");
        await notificationRepository.SaveChangesAsync(cancellationToken);

        return result.Value;
    }
}
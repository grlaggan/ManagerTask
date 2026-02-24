using ManagerTask.Application.Jobs;
using ManagerTask.Domain.Entities.Events;
using MediatR;
using Quartz;

namespace ManagerTask.Application.Handlers.Events;

public class NotificationCreatedEventHandler(ISchedulerFactory schedulerFactory)
    : INotificationHandler<NotificationCreatedDomainEvent>
{

    public async Task Handle(NotificationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        Console.WriteLine("hui");
        await JobFactory.CreateNotificationJob(notification.Notification, scheduler, notification.User.ChatId);
    }
}
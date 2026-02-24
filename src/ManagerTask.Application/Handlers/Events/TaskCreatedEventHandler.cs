using ManagerTask.Application.Jobs;
using ManagerTask.Domain.Entities.Events;
using MediatR;
using Quartz;

namespace ManagerTask.Application.Handlers.Events;

public class TaskCreatedEventHandler(ISchedulerFactory schedulerFactory) : INotificationHandler<TaskCreatedDomainEvent>
{
    public async Task Handle(TaskCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

        var task = notification.Task;
        var tableName = notification.TableName;

        if (task.SendTime - DateTime.UtcNow > TimeSpan.FromDays(1))
        {
            await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId, minutes: 5);
            await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId, hours: 5);
            await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId, days: 1);
            
        } else if (task.SendTime - DateTime.UtcNow < TimeSpan.FromDays(1) &&
                   task.SendTime - DateTime.UtcNow > TimeSpan.FromHours(5))
        {
            await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId, minutes: 5);
            await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId, hours: 5);
        }
        else if (task.SendTime - DateTime.UtcNow < TimeSpan.FromHours(1) &&
                 task.SendTime - DateTime.UtcNow > TimeSpan.FromMinutes(5))
        {
            await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId, minutes: 5);
        }
        
        await JobFactory.CreateJob(task, tableName, scheduler, notification.User.ChatId);
        
    }
}
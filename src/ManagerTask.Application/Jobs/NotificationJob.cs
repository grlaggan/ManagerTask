using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using Quartz;

namespace ManagerTask.Application.Jobs;

public class NotificationJob : IJob
{
    private readonly IRabbitMqService _rabbitMqService;

    public NotificationJob(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var notification = new NotificationCreatedNotification
        (
            context.MergedJobDataMap.GetString("Name")!,
            context.MergedJobDataMap.GetString("Message")!,
            context.MergedJobDataMap.GetString("ChatId")!,
            context.MergedJobDataMap.GetInt("Minutes"),
            context.MergedJobDataMap.GetInt("Hours"),
            context.MergedJobDataMap.GetInt("Days")
        );
        await _rabbitMqService.NotificationPublish(notification);
    }
}
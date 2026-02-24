using System.Threading.Tasks;
using ManagerTask.Application.Abstracts;
using Quartz;

namespace ManagerTask.Application.Jobs;

public class TaskNotificationJob : IJob
{
    private readonly IRabbitMqService _rabbitMqService;

    public TaskNotificationJob(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var notification = new TaskCreatedNotification
        (
            context.MergedJobDataMap.GetString("Name")!,
            context.MergedJobDataMap.GetString("Description")!,
            context.MergedJobDataMap.GetString("TableName")!,
            context.MergedJobDataMap.GetString("ChatId")!,
            context.MergedJobDataMap.GetInt("Minutes"),
            context.MergedJobDataMap.GetInt("Hours"),
            context.MergedJobDataMap.GetInt("Days")
        );
        await _rabbitMqService.Publish(notification);
    }

}

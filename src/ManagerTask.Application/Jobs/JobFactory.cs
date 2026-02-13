using Quartz;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Application.Jobs;

public static class JobFactory
{
    public static async Task CreateJob(TaskEntity task, string tableName, IScheduler scheduler,
        int minutes = 0, int hours = 0, int days = 0)
    {
        DateTime time = task.SendTime;
        if (minutes > 0) time = task.SendTime.AddMinutes(-minutes); 
        if (hours > 0) time = task.SendTime.AddHours(-hours); 
        if (days > 0) time = task.SendTime.AddDays(-days); 
        
        
        var job = JobBuilder.Create<TaskNotificationJob>()
            .WithIdentity($"TaskNotification-{Guid.NewGuid()}")
            .UsingJobData("Name", task.Name)
            .UsingJobData("Description", task.Description)
            .UsingJobData("TableName", tableName)
            .UsingJobData("Minutes", minutes)
            .UsingJobData("Hours", hours)
            .UsingJobData("Days", days).Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"TaskNotificationTrigger-{Guid.NewGuid()}")
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
namespace ManagerTask;

public class RabbitOptions
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Exchange { get; set; } = "tasks.notification.exchange";
    public string QueueName { get; set; } = "tasks.notification.queue";
    public string NotificationQueueName { get; set; } = "notifications.notification.queue";
}

namespace ManagerTask.Application.Abstracts;

public interface IRabbitMqService : IDisposable
{
    Task Publish(TaskCreatedNotification notification);
}

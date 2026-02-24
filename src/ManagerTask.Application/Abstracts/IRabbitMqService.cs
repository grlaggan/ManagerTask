using System;
using System.Threading.Tasks;
using ManagerTask.Application.Models.Dtos;

namespace ManagerTask.Application.Abstracts;

public interface IRabbitMqService : IDisposable
{
    Task Publish(TaskCreatedNotification notification);
    Task NotificationPublish(NotificationCreatedNotification notification);
}

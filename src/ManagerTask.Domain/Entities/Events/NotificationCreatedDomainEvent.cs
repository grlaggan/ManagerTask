using ManagerTask.Domain.Entities.NotificationEntity;

namespace ManagerTask.Domain.Entities.Events;

public class NotificationCreatedDomainEvent : IDomainEvent
{
    public Notification Notification { get; set; } = null!;
}
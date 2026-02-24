using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Domain.Entities.Events;

public class NotificationCreatedDomainEvent : IDomainEvent
{
    public Notification Notification { get; set; } = null!;
    public User User { get; set; } = null!;
}
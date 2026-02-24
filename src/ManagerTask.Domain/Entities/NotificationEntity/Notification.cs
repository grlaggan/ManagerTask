using FluentResults;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.BaseEntity;
using ManagerTask.Domain.Entities.Events;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Domain.Entities.NotificationEntity;

public class Notification : Entity
{
    public string Name { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime NotificationTime { get; set; }
    public User User { get; set; }

    public Notification(Guid id, string name, string message, DateTime notificationTime)
    {
        Id = id;
        Name = name;
        Message = message;
        CreatedAt = DateTime.UtcNow;
        NotificationTime = notificationTime;
    }

    public static Result<Notification> Create(string name, string message, DateTime notificationTime, User user)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Notification.NotificationNameEmpty,
                "Notification name cannot be empty!"));
        
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Notification.NotificationMessageEmpty,
                "Notification message cannot be empty!"));

        if (DateTime.UtcNow > notificationTime)
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Notification.NotificationTimeInvalid,
                "Notification time is invalid!"));
        
        var notification = new Notification(Guid.NewGuid(), name, message, notificationTime) {User = user};
        
        notification.RaiseDomainEvent(new NotificationCreatedDomainEvent
        {
            Notification = notification,
            User = user
        });
        
        return notification;
    }
}
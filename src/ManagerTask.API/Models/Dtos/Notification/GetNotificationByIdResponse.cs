namespace ManagerTask.Models.Dtos.Notification;

public record GetNotificationByIdResponse(string Name, string Message, DateTime NotificationTime, DateTime CreatedAt);
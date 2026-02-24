namespace ManagerTask.Models.Dtos.Task;

public record CreateNotificationRequest(string Name, string Message, DateTime NotificationTime);
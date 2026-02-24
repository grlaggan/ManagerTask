namespace ManagerTask.Application.Models.Dtos;

public record NotificationCreatedNotification(string Name, string Message, string ChatId, int Minutes, int Hours, int Days);
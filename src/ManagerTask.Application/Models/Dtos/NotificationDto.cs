namespace ManagerTask.Application.Models.Dtos;

public record NotificationDto(Guid Id, string Name, string Message, DateTime NotificationTime, DateTime CreatedAt);
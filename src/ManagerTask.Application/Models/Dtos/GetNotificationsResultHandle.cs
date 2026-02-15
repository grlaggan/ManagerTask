namespace ManagerTask.Application.Models.Dtos;

public record GetNotificationsResultHandle(List<NotificationDto> Notifications, int CountPages);
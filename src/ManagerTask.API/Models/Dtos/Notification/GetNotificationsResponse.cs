using ManagerTask.Application.Models.Dtos;

namespace ManagerTask.Models.Dtos.Notification;

public record GetNotificationsResponse(List<NotificationDto> Notifications, int Page, int Offset, int CountPages);
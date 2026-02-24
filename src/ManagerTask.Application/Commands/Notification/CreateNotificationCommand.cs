using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Notification;

public record CreateNotificationCommand(string Name, string Message, DateTime NotificationTime, string ChatId) : IRequest<Result<Guid>>;
using FluentResults;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetNotificationByIdQuery(Guid Id) : IRequest<Result<NotificationDto>>;
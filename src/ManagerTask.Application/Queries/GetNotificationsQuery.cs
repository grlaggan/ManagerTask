using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetNotificationsQuery(PaginationParams @Params, string ChatId) : IRequest<Result<GetNotificationsResultHandle>>;
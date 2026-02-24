using FluentResults;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetUserByChatIdQuery(string ChatId) : IRequest<Result<UserDto>>;
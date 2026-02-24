using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.User;

public record CreateUserCommand(string ChatId) : IRequest<Result<Guid>>;
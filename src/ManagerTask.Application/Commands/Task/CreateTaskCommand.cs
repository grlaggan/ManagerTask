using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record CreateTaskCommand(string Name,
    string Description,
    Guid TableId,
    DateTime SendTime) : IRequest<Result<Guid>>;
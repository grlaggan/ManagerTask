using System;
using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record UpdateTaskCommand(Guid TaskId, string Name, string Description, Guid TableId, DateTime SendTime) : IRequest<Result<Guid>>;

using System;
using FluentResults;
using ManagerTask.Domain.Entities.UserEntity;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record CreateTaskCommand(string Name,
    string Description,
    string ChatId,
    Guid TableId,
    DateTime SendTime) : IRequest<Result<Guid>>;
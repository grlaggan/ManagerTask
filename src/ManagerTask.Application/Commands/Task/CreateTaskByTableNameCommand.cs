using System;
using FluentResults;
using ManagerTask.Domain.Entities.UserEntity;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record CreateTaskByTableNameCommand(string Name, string Description, string TableName, DateTime SendTime, string ChatId) : IRequest<Result<Guid>>;

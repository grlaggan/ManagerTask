using System;
using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Table;

public record CreateTableCommand(string Name, string Description, string ChatId) : IRequest<Result<Guid>>;

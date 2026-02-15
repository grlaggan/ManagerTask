using System;
using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Table;

public record UpdateTableCommand(Guid TableId, string Name, string Description) : IRequest<Result<Guid>>;

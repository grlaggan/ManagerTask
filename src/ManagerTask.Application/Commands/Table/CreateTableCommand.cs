using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Table;

public record CreateTableCommand(string Name, string Description) : IRequest<Result<Guid>>;

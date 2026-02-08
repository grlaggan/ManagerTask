using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record CreateTaskByTableNameCommand(string Name, string Description, string TableName, DateTime SendTime) : IRequest<Result<Guid>>;

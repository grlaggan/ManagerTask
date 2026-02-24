using System;
using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record UpdateStatusFailedTaskCommand(Guid Id) : IRequest<Result>;
    

using System;
using FluentResults;
using MediatR;

namespace ManagerTask.Application.Commands.Task;

public record UpdateStatusTaskCommand(Guid Id) : IRequest<Result>;
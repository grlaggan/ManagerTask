using FluentResults;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetTaskQuery(Guid Id) : IRequest<Result<TaskDto>>;
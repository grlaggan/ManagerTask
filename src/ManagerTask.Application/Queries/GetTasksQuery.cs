using FluentResults;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Models.Profiles;

public record GetTasksQuery() : IRequest<Result<List<TaskDto>>>;

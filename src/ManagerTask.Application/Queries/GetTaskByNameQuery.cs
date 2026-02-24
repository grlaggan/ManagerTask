using FluentResults;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetTaskByNameQuery(string Name) : IRequest<Result<TaskDto>>;
    

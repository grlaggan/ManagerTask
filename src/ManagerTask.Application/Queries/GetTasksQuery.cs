using System.Collections.Generic;
using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetTasksQuery(string? TableName, string ChatId, PaginationParams PaginationParams) : IRequest<Result<GetTasksResultHandle>>;

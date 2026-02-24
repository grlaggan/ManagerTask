using System.Collections.Generic;
using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Application.Models.Dtos;
using MediatR;

namespace ManagerTask.Application.Queries;

public record GetTablesQuery(PaginationParams @Params, string ChatId) : IRequest<Result<GetTablesResultHandle>>;

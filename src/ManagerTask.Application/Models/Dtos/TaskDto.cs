using System;
using ManagerTask.Domain.Entities.TaskEntity;

namespace ManagerTask.Application.Models.Dtos;

public record TaskDto(Guid Id, string Name, string Description, TableDto Table, DateTime SendTime, DateTime CreatedAt,
    StatusTask Status);

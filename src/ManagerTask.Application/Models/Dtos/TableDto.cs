using System;
using ManagerTask.Domain.Entities.TaskEntity;

namespace ManagerTask.Application.Models.Dtos;

public record TableDto(Guid Id, string Name, string Description);

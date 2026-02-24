using ManagerTask.Domain.Entities.TaskEntity;

namespace ManagerTask.Models.Dtos.Task;

public record GetTaskResponse(Guid Id, string Name, string Description, string TableName, StatusTask Status);
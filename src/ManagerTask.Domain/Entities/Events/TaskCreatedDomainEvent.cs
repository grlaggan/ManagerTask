using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Domain.Entities.Events;

public class TaskCreatedDomainEvent : IDomainEvent
{
    public Task Task { get; set; } = null!;
    public string TableName { get; set; } = string.Empty;
}
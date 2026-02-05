namespace ManagerTask.Dtos.Task;

public record CreateTaskRequest(string Name, string Description, Guid TableId, DateTime SendTime);
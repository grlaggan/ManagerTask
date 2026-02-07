namespace ManagerTask;

public record UpdateTaskRequest(string Name, string Description, Guid TableId, DateTime SendTime);

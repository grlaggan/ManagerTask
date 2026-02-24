namespace ManagerTask;

public record CreateTaskByTableNameRequest(string Name, string Description, string TableName, DateTime SendTime);

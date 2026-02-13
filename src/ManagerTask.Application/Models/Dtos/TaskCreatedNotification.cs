namespace ManagerTask;

public record TaskCreatedNotification(string Name, string Description, string TableName, int Minutes, int Hours, int Days);

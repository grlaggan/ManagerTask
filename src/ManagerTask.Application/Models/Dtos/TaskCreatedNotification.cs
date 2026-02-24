namespace ManagerTask;

public record TaskCreatedNotification(string Name, string Description, string TableName, string ChatId, int Minutes, int Hours, int Days);

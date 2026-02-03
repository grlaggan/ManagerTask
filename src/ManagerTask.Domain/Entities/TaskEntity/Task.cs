using FluentResults;
using ManagerTask.Domain.Entities.TableEntity;

namespace ManagerTask.Domain.Entities.TaskEntity;

public class Task
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Table Table { get; private set; } 
    public DateTime CreatedAt { get; private set; }
    public DateTime SendTime { get; private set; }
    public StatusTask Status { get; private set; }

    public Task(Guid id, string name, string description, DateTime sendTime, Table table)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        SendTime = sendTime;
        Table = table;
        Status = StatusTask.Pending;
    }
    
    public static Result<Task> Create(string name, string description, DateTime sendTime, Table? table) 
    {
        var validationResult = ValidationData(name, description, sendTime);
        
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        if (table is null)
            return Result.Fail("Table cannot be null.");
        
        var task = new Task(Guid.NewGuid(), name, description, sendTime, table);
        return task;
    }

    private static Result ValidationData(string name, string description, DateTime sendTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Task name cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail("Task description cannot be empty.");
        
        if (sendTime < DateTime.UtcNow)
            return Result.Fail("Send time cannot be in the past.");

        return Result.Ok();
    }
}
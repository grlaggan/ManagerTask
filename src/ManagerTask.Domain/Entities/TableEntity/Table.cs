using FluentResults;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Domain.Entities.TableEntity;

public class Table
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<TaskEntity.Task> Tasks { get; private set; } = [];

    public Table(Guid id, string name, string description, List<TaskEntity.Task>? tasks = null)
    {
        Id = id;
        Name = name;
        Description = description;
        
        if (tasks is not null && tasks.Count != 0)
            Tasks = tasks;
    }
    
    public static Result<Table> Create(string name, string description) 
    {
        var validationResult = ValidationData(name, description);
        
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var table = new Table(Guid.NewGuid(), name, description);
        return table;
    }
    
    public static Result<Table> Create(string name, string description, List<TaskEntity.Task> tasks) 
    {
        var validationResult = ValidationData(name, description);
        
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var table = new Table(Guid.NewGuid(), name, description, tasks);
        return table;
    }

    public Result AddTask(Task? task)
    {
        if (task is null)
        {
            return Result.Fail("Task cannot be null.");
        }
        
        Tasks.Add(task);
        return Result.Ok();
    }
    
    private static Result ValidationData(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Table name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail("Table description cannot be empty.");
        
        return Result.Ok();
    }
}
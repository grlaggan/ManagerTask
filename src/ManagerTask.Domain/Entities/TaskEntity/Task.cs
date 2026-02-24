using FluentResults;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.BaseEntity;
using ManagerTask.Domain.Entities.Events;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Domain.Entities.TaskEntity;

public class Task : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Table Table { get; set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime SendTime { get; set; }
    public User User { get; set; }
    public StatusTask Status { get; set; }

    public Task(Guid id, string name, string description, DateTime sendTime)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        SendTime = sendTime;
        Status = StatusTask.Pending;
    }

    public static Result<Task> Create(string name, string description, DateTime sendTime, User user, Table? table)
    {
        var validationResult = ValidationData(name, description, sendTime);

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        if (table is null)
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Task.TaskTableNull, "Table cannot be null."));

        var task = new Task(Guid.NewGuid(), name, description, sendTime)
        {
            Table = table,
            User = user
        };
        
        task.RaiseDomainEvent(new TaskCreatedDomainEvent
        {
            Task = task,
            TableName = table.Name,
            User = user
        });
        
        return task;
    }

    private static Result ValidationData(string name, string description, DateTime sendTime)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail(
                ApplicationError.Validation(ErrorCodes.Task.TaskNameEmpty, "Task name cannot be empty."));
        }


        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Task.TaskDescriptionEmpty, "Task description cannot be empty."));

        if (sendTime < DateTime.UtcNow)
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Task.TaskSendTimePast, "Send time cannot be in the past."));

        return Result.Ok();
    }
}
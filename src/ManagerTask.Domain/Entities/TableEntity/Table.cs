using FluentResults;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.BaseEntity;
using ManagerTask.Domain.Entities.UserEntity;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Domain.Entities.TableEntity;

public class Table: Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public User User { get; set; }
    public List<TaskEntity.Task> Tasks { get; set; } = [];

    public Table(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static Result<Table> Create(string name, string description, User user)
    {
        var validationResult = ValidationData(name, description);

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var table = new Table(Guid.NewGuid(), name, description){User = user};
        return table;
    }

    public static Result<Table> Create(string name, string description, List<TaskEntity.Task> tasks, User user)
    {
        var validationResult = ValidationData(name, description);

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        var table = new Table(Guid.NewGuid(), name, description){User = user};
        var addTasksResult = table.AddTasks(tasks);

        if (addTasksResult.IsFailed)
            return Result.Fail(addTasksResult.Errors);

        return table;
    }

    public Result AddTask(Task? task)
    {
        if (task is null)
        {
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Table.TableTasksNull, "Task cannot be null"));
        }

        Tasks.Add(task);
        return Result.Ok();
    }

    private static Result ValidationData(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Table.TableNameEmpty, "Table name cannot be empty."));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Table.TableDescriptionEmpty, "Table description cannot be empty."));

        return Result.Ok();
    }

    private Result AddTasks(List<Task>? tasks)
    {
        if (tasks is null || tasks.Count == 0)
        {
            return Result.Fail(ApplicationError.Validation(ErrorCodes.Table.TableTasksNull, "Tasks list cannot be null or empty."));
        }

        Tasks.AddRange(tasks);
        return Result.Ok();
    }
}
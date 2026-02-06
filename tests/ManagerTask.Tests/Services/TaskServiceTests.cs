using ManagerTask.Domain.Entities.TableEntity;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public void CreateTask_ShouldCreateTaskSuccessfully()
    {
        var testTable = new Table(Guid.NewGuid(), "Main", "Main Table");

        var result = Task.Create("Test Task", "This is a test task", DateTime.UtcNow.AddDays(1),
            testTable);
        Assert.True(result.IsSuccess);
        Assert.Equal("Test Task", result.Value.Name);
        Assert.Equal("This is a test task", result.Value.Description);
    }

    [Fact]
    public void CreateTask_ShouldFail_WhenSendTimeIsInThePast()
    {
        var testTable = new Table(Guid.NewGuid(), "Main", "Main Table");

        var result = Task.Create("Test Task", "This is a test task", DateTime.UtcNow.AddDays(-1),
            testTable);
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Send time cannot be in the past.");
    }
}
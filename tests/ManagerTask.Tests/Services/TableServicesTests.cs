using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Domain.Entities.TaskEntity;
using Task = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Tests.Services;

public class TableServicesTests
{
    [Fact]
    public void CreateTable_ShouldCreateTableSuccessfully()
    {
        var result = Table.Create("Main", "Main Table");
        
        Assert.True(result.IsSuccess);
        Assert.Equal("Main", result.Value.Name);
        Assert.Equal("Main Table", result.Value.Description);
    }
    
    [Fact]
    public void CreateTable_WithTask_ShouldCreateTableSuccessfully()
    {
        var testTable = Table.Create("Main", "Main Table").Value;
        
        var result = Table.Create("Secondary", "Secondary Table", new List<Task>
        {
            Task.Create("Test Task", "This is a test task", DateTime.UtcNow.AddDays(1),
                testTable).Value
        });
        
        Assert.True(result.IsSuccess);
        Assert.Equal("Secondary", result.Value.Name);
        Assert.Equal("Secondary Table", result.Value.Description);
        Assert.Single(result.Value.Tasks);
        Assert.Equal("Test Task", result.Value.Tasks[0].Name);
    }
}
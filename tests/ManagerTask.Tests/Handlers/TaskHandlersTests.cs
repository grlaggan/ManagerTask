using ManagerTask.Application.Abstracts;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;
using Moq;
using FluentResults;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Application.Handlers.Task;
using ManagerTask.Application.Commands.Task;

namespace ManagerTask.Tests.Handlers;

public class TaskHandlersTests
{
    [Fact]
    public async Task CreateTaskHandler_Handle_Success()
    {
        // Arrange
        var mockTaskRepository = new Mock<ITaskRepository>();
        var mockTableRepository = new Mock<ITableRepository>();
        var mockTransactionManager = new Mock<ITransactionManager>();
        var mockTransactionScope = new Mock<ITransactionScope>();

        var tableId = Guid.NewGuid();
        var table = Table.Create("Test Table", "This is a test table").Value;
        var taskResult = TaskEntity.Create("Test Task", "This is a test task", DateTime.UtcNow.AddDays(1), table).Value;

        mockTransactionScope
            .Setup(ts => ts.Commit())
            .Returns(Result.Ok());

        mockTableRepository.Setup(repo => repo.GetByIdAsync(tableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(table));

        mockTaskRepository.Setup(repo => repo.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<TaskEntity>("Task not found"));

        mockTaskRepository.Setup(repo => repo.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Guid.NewGuid()));

        mockTransactionManager.Setup(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(mockTransactionScope.Object));

        var handler = new CreateTaskHandler(mockTaskRepository.Object, mockTransactionManager.Object, mockTableRepository.Object);

        var command = new CreateTaskCommand("Test Task2", "This is a test task", tableId, DateTime.UtcNow.AddDays(1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskHandler_Handler_Failure()
    {
        // Arrange
        var mockTaskRepository = new Mock<ITaskRepository>();
        var mockTableRepository = new Mock<ITableRepository>();
        var mockTransactionManager = new Mock<ITransactionManager>();
        var mockTransactionScope = new Mock<ITransactionScope>();

        var tableId = Guid.NewGuid();
        var table = Table.Create("Test Table", "This is a test table").Value;
        var taskResult = TaskEntity.Create("Test Task", "This is a test task", DateTime.UtcNow.AddDays(1), table).Value;

        mockTransactionScope
            .Setup(ts => ts.Commit())
            .Returns(Result.Ok());

        mockTableRepository.Setup(repo => repo.GetByIdAsync(tableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(table));

        mockTaskRepository.Setup(repo => repo.GetByNameAsync("Test Task", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(taskResult));

        mockTaskRepository.Setup(repo => repo.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Guid.NewGuid()));

        mockTransactionManager.Setup(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(mockTransactionScope.Object));

        var handler = new CreateTaskHandler(mockTaskRepository.Object, mockTransactionManager.Object, mockTableRepository.Object);

        var command = new CreateTaskCommand("Test Task", "This is a test task", tableId, DateTime.UtcNow.AddDays(1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

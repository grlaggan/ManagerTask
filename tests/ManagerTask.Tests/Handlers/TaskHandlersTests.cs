using ManagerTask.Application.Abstracts;
using Moq;
using FluentResults;
using ManagerTask.Domain.Entities.TableEntity;
using ManagerTask.Application.Handlers.TaskHandlers;
using ManagerTask.Application.Commands.Task;

namespace ManagerTask.Tests.Handlers;

public class TaskHandlersTests
{
    [Fact]
    public async Task UpdateTaskHandler_Handle_Success()
    {
        // Arrange
        var mockTaskRepository = new Mock<ITaskRepository>();
        var mockTableRepository = new Mock<ITableRepository>();
        var mockTransactionManager = new Mock<ITransactionManager>();
        var mockTransactionScope = new Mock<ITransactionScope>();

        var tableId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var table = Table.Create("Test Table", "This is a test table").Value;

        mockTransactionScope
            .Setup(ts => ts.Rollback())
            .Returns(Result.Ok());

        mockTransactionManager
            .Setup(tm => tm.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        mockTransactionScope
            .Setup(ts => ts.Commit())
            .Returns(Result.Ok());

        mockTableRepository.Setup(repo => repo.GetByIdAsync(tableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(table));

        mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<string>(), It.IsAny<string>(), table, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(taskId));

        mockTransactionManager.Setup(tm => tm.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(mockTransactionScope.Object));

        var handler = new UpdateTaskHandler(mockTaskRepository.Object, mockTableRepository.Object, mockTransactionManager.Object);

        var command = new UpdateTaskCommand(taskId, "Updated Task", "This is an updated task", tableId, DateTime.UtcNow.AddDays(2));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(taskId, It.IsAny<string>(), It.IsAny<string>(), table, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

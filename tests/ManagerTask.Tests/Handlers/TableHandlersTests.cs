using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Table;
using ManagerTask.Application.Handlers.Table;
using ManagerTask.Domain.Entities.TableEntity;
using Moq;

namespace ManagerTask.Tests.Handlers;

public class TableHandlersTests
{
    [Fact]
    public async Task CreateTableHandler_Handle_Success()
    {
        // Arrange
        var repositoryMock = new Mock<ITableRepository>();
        repositoryMock
            .Setup(r => r.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("Not found"));
        repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Guid.NewGuid()));
        repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var handler = new CreateTableHandler(repositoryMock.Object);
        var command = new CreateTableCommand("Test Table", "Test Description");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTableHandler_Handle_Failure_WhenTableExists()
    {
        // Arrange
        var tableResult = Table.Create("Existing Table", "Description");

        var repositoryMock = new Mock<ITableRepository>();
        repositoryMock
            .Setup(r => r.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(tableResult.Value));
        repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Guid.NewGuid()));
        repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var handler = new CreateTableHandler(repositoryMock.Object);
        var command = new CreateTableCommand("Test Table", "Test Description");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Table>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTableHandler_Handle_Success()
    {
        // Arrange
        var table = Table.Create("Test Table", "Test Description").Value;
        var repositoryMock = new Mock<ITableRepository>();
        var transactionManagerMock = new Mock<ITransactionManager>();
        var transactionScopeMock = new Mock<ITransactionScope>();

        transactionManagerMock
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(transactionScopeMock.Object));

        transactionManagerMock
            .Setup(t => t.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        transactionScopeMock
            .Setup(ts => ts.Commit())
            .Returns(Result.Ok());

        transactionScopeMock
            .Setup(ts => ts.Rollback())
            .Verifiable();

        repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(table));

        repositoryMock
            .Setup(r => r.UpdateTableAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Guid.NewGuid()));

        var handler = new UpdateTableHandler(repositoryMock.Object, transactionManagerMock.Object);
        var command = new UpdateTableCommand(table.Id, "Updated Test Table", "Updated Test Description");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        repositoryMock.Verify(r => r.UpdateTableAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        transactionManagerMock.Verify(t => t.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionScopeMock.Verify(ts => ts.Commit(), Times.Once);
    }
}

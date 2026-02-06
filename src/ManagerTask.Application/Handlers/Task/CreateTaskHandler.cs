using FluentResults;
using ManagerTask.Application.Abstracts;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;
using ManagerTask.Application.Commands.Task;
using MediatR;
using ManagerTask.Domain.Common.Errors;

namespace ManagerTask.Application.Handlers.Task;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ITableRepository _tableRepository;

    public CreateTaskHandler(ITaskRepository repository, ITransactionManager transactionManager,
     ITableRepository tableRepository)
    {
        _taskRepository = repository;
        _transactionManager = transactionManager;
        _tableRepository = tableRepository;
    }

    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var transactionBeginResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionBeginResult.IsFailed)
            return Result.Fail(transactionBeginResult.Errors[0]);

        using var transactionScope = transactionBeginResult.Value;

        var resultGetByName = await _taskRepository.GetByNameAsync(request.Name, cancellationToken);

        if (resultGetByName.IsSuccess)
        {
            transactionScope.Rollback();
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Task.TaskAlreadyExists, "Task with the same name already exists"));
        }

        var tableResult = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (tableResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(tableResult.Errors[0]);
        }

        var taskResult = TaskEntity.Create(request.Name, request.Description, request.SendTime, tableResult.Value);

        if (taskResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(taskResult.Errors[0]);
        }


        var result = await _taskRepository.CreateAsync(taskResult.Value, cancellationToken);

        if (result.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(result.Errors[0]);
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(saveChangesResult.Errors[0]);
        }

        var resultCommit = transactionScope.Commit();

        if (resultCommit.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultCommit.Errors[0]);
        }

        return result.Value;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.Table;
using TableEntity = ManagerTask.Domain.Entities.TableEntity.Table;
using MediatR;
using ManagerTask.Domain.Common.Errors;

namespace ManagerTask.Application.Handlers.Table;

public class CreateTableHandler(
    ITableRepository repository,
    IUserRepository userRepository,
    ITransactionManager transactionManager) : IRequestHandler<CreateTableCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var resultBeginTransaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        
        if (resultBeginTransaction.IsFailed)
            return Result.Fail(resultBeginTransaction.Errors[0]);

        using var transactionScope = resultBeginTransaction.Value;

        var getTableByNameResult = await repository.GetByNameAsync(request.Name, cancellationToken);

        if (getTableByNameResult.IsSuccess)
        {
            transactionScope.Rollback();
            return Result.Fail(ApplicationError.Conflict(ErrorCodes.Table.TableAlreadyExists, "Table with the same name already exists"));
        }

        var resultGetUserByChatId = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultGetUserByChatId.Errors[0]);
        }

        var user = resultGetUserByChatId.Value;

        var tableResult = TableEntity.Create(request.Name, request.Description, user);

        if (tableResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(tableResult.Errors[0]);
        }

        var createResult = await repository.CreateAsync(tableResult.Value, cancellationToken);

        if (createResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(createResult.Errors[0]);
        }

        var saveResult = await repository.SaveAsync(cancellationToken);

        if (saveResult.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(saveResult.Errors[0]);
        }

        var resultCommit = transactionScope.Commit();

        if (resultCommit.IsFailed)
        {
            transactionScope.Rollback();
            return Result.Fail(resultCommit.Errors[0]);
        }

        return tableResult.Value.Id;
    }

}

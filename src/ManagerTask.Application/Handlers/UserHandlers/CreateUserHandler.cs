using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Commands.User;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.UserEntity;
using MediatR;

namespace ManagerTask.Application.Handlers.UserHandlers;

public class CreateUserHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var resultCreateUser = User.Create(request.ChatId);

        if (resultCreateUser.IsFailed)
            return Result.Fail(resultCreateUser.Errors[0]);

        var result = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (result.IsSuccess)
            return Result.Fail(ApplicationError
                .Conflict(ErrorCodes.User.UserAlreadyExists, "User already exists"));

        var resultAddUserToDb = await userRepository.CreateUserAsync(resultCreateUser.Value, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        
        if (resultAddUserToDb.IsFailed)
            return Result.Fail(resultAddUserToDb.Errors[0]);

        return resultAddUserToDb.Value;
    }
}
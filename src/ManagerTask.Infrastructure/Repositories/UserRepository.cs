using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace ManagerTask.Infrastructure.Repositories;

public class UserRepository(IApplicationDbContext context) : IUserRepository
{
    public async Task<Result<User>> GetUserByChatIdAsync(string chatId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId, cancellationToken);

        if (user is null)
            return Result.Fail(ApplicationError
                .NotFound(ErrorCodes.User.UserNotFound, "User was not found"));

        return user;
    }

    public async Task<Result<Guid>> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
        return user.Id;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var saveChangesAsync = await context.SaveChangesAsync(cancellationToken);
        return saveChangesAsync;
    }
}
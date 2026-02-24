using FluentResults;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Application.Abstracts;

public interface IUserRepository
{
    public Task<Result<User>> GetUserByChatIdAsync(string chatId, CancellationToken cancellationToken);
    public Task<Result<Guid>> CreateUserAsync(User user, CancellationToken cancellationToken);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
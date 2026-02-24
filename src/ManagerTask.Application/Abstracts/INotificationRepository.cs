using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Application.Abstracts;

public interface INotificationRepository
{
    public Task<Result<Guid>> CreateAsync(Notification notification, CancellationToken cancellationToken);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    public Task<Result<List<Notification>>> GetAllAsync(PaginationParams @params, User user, CancellationToken cancellationToken);
    public Task<Result<Notification>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<int> GetCountAsync(User user, CancellationToken cancellationToken);
}
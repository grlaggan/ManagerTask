using FluentResults;
using ManagerTask.Application.Common;
using ManagerTask.Domain.Entities.NotificationEntity;

namespace ManagerTask.Application.Abstracts;

public interface INotificationRepository
{
    public Task<Result<Guid>> CreateAsync(Notification notification, CancellationToken cancellationToken);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    public Task<Result<List<Notification>>> GetAllAsync(PaginationParams @params, CancellationToken cancellationToken);
    public Task<int> GetCountAsync(CancellationToken cancellationToken);
}
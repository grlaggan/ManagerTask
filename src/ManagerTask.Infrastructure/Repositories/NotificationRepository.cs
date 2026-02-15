using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Common;
using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ManagerTask.Infrastructure.Repositories;

public class NotificationRepository(IApplicationDbContext context) : INotificationRepository
{

    public async Task<Result<Guid>> CreateAsync(Notification notification,
        CancellationToken cancellationToken)
    {
        await context.Notifications.AddAsync(notification, cancellationToken);

        return notification.Id;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var saveChangesAsync = await context.SaveChangesAsync(cancellationToken);
        return saveChangesAsync;
    }

    public async Task<Result<List<Notification>>> GetAllAsync(PaginationParams @params, CancellationToken cancellationToken)
    {
        var notifications = await context.Notifications
            .Page(@params)
            .ToListAsync(cancellationToken);

        return notifications;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        var count = await context.Notifications.CountAsync(cancellationToken);

        return count;
    }
}
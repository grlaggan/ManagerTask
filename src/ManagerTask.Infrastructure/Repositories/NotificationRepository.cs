using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Common;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.NotificationEntity;
using ManagerTask.Domain.Entities.UserEntity;
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

    public async Task<Result<List<Notification>>> GetAllAsync(PaginationParams @params, User user, CancellationToken cancellationToken)
    {
        var notifications = await context.Notifications
            .Include(n => n.User)
            .Where(n => n.User.ChatId == user.ChatId)
            .Page(@params)
            .ToListAsync(cancellationToken);

        return notifications;
    }

    public async Task<Result<Notification>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        if (notification == null)
            return Result.Fail(ApplicationError
                .NotFound(ErrorCodes.Notification.NotificationIsNull, "Notification was not found!"));

        return notification;
    }

    public async Task<int> GetCountAsync(User user, CancellationToken cancellationToken)
    {
        var count = await context.Notifications
            .Include(n => n.User)
            .Where(n => n.User.ChatId == user.ChatId)
            .CountAsync(cancellationToken);

        return count;
    }
}
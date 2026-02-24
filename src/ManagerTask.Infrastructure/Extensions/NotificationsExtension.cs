using ManagerTask.Application.Common;
using ManagerTask.Domain.Entities.NotificationEntity;

namespace ManagerTask.Infrastructure.Extensions;

public static class NotificationsExtension
{
    extension(IQueryable<Notification> query)
    {
        public IQueryable<Notification> Page(PaginationParams @params)
        {
            var page = @params.Page ?? 1;
            var offset = @params.Offset ?? 3;
            var skip = (page - 1) * offset;

            return query.Skip(skip).Take(offset);
        }
    }
}
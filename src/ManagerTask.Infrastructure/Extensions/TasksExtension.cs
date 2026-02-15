using ManagerTask.Application.Common;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Infrastructure.Extensions;

public static class TasksExtension
{
    extension(IQueryable<TaskEntity> query)
    {
        public IQueryable<TaskEntity> Page(PaginationParams paginationParams)
        {
            var page = paginationParams.Page ?? 1;
            var offset = paginationParams.Offset ?? 3;
            var skip = (page - 1) * offset;
            
            return query.Skip(skip).Take(offset);
        }
    }
}
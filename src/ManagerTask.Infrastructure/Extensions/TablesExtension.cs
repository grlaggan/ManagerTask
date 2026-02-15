using ManagerTask.Application.Common;
using ManagerTask.Domain.Entities.TableEntity;

namespace ManagerTask.Infrastructure.Extensions;

public static class TablesExtension
{
    extension(IQueryable<Table> query)
    {
        public IQueryable<Table> Page(PaginationParams @params)
        {
            var page = @params.Page ?? 1;
            var offset = @params.Offset ?? 3;
            var skip = (page - 1) * offset;

            return query.Skip(skip).Take(offset);
        }
    }
}
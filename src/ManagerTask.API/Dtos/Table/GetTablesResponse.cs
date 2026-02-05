using ManagerTask.Application.Models.Dtos;

namespace ManagerTask;

public record GetTablesResponse(List<TableDto> Tables);

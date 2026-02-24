using ManagerTask.Application.Models.Dtos;

namespace ManagerTask;

public record GetTablesResponse(List<TableDto> Tables, int Page, int Offset, int CountPages);

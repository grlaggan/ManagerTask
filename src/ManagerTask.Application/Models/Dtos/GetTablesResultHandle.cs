namespace ManagerTask.Application.Models.Dtos;

public record GetTablesResultHandle(List<TableDto> Tables, int CountPages);
namespace ManagerTask.Application.Models.Dtos;

public record TaskDto(Guid Id, string Name, string Description, TableDto Table, DateTime SendTime, DateTime CreatedAt);

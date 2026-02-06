using ManagerTask.Application.Models.Dtos;

namespace ManagerTask;

public record GetTasksResponse(string Detail, List<TaskDto> Tasks);

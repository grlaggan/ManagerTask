using ManagerTask.Application.Models.Dtos;

namespace ManagerTask;

public record GetTasksResponse(string Detail, List<TaskDto> Tasks, int Page, int Offset, int CountPages);

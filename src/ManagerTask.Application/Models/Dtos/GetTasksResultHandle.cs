using System.Collections.Generic;

namespace ManagerTask.Application.Models.Dtos;

public record GetTasksResultHandle(List<TaskDto> Tasks, int CountPages);
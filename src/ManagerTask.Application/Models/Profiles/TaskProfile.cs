using AutoMapper;
using ManagerTask.Application.Models.Dtos;
using TaskEntity = ManagerTask.Domain.Entities.TaskEntity.Task;

namespace ManagerTask.Application.Models.Profiles;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskEntity, TaskDto>();
    }
}

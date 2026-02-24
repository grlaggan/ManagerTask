using AutoMapper;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Domain.Entities.NotificationEntity;

namespace ManagerTask.Application.Models.Profiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
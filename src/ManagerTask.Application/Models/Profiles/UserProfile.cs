using AutoMapper;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Domain.Entities.UserEntity;

namespace ManagerTask.Application.Models.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}
using AutoMapper;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Domain.Entities.TableEntity;

namespace ManagerTask.Application.Models.Profiles;

public class TableProfile : Profile
{
    public TableProfile()
    {
        CreateMap<Table, TableDto>();
    }
}

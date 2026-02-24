using AutoMapper;
using FluentResults;
using ManagerTask.Application.Abstracts;
using ManagerTask.Application.Models.Dtos;
using ManagerTask.Application.Queries;
using ManagerTask.Domain.Entities.UserEntity;
using MediatR;

namespace ManagerTask.Application.Handlers.UserHandlers;

public class GetUserByChatIdHandler(
    IUserRepository userRepository,
    IMapper mapper)
    : IRequestHandler<GetUserByChatIdQuery, Result<UserDto>>
{

    public async Task<Result<UserDto>> Handle(GetUserByChatIdQuery request, CancellationToken cancellationToken)
    {
        var resultGetUserByChatId = await userRepository.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (resultGetUserByChatId.IsFailed)
            return Result.Fail(resultGetUserByChatId.Errors[0]);

        return mapper.Map<UserDto>(resultGetUserByChatId.Value);
    }
}
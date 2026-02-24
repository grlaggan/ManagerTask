using FluentResults;
using ManagerTask.Domain.Common.Errors;
using ManagerTask.Domain.Entities.BaseEntity;

namespace ManagerTask.Domain.Entities.UserEntity;

public class User : Entity
{
    public string ChatId { get; set; } = null!;

    public static Result<User> Create(string chatId)
    {
        if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(chatId))
            return Result.Fail(ApplicationError
                .Validation(ErrorCodes.User.UserChatItEmpty, "Chat id cannot be empty"));
        
        return new User{Id = Guid.NewGuid(), ChatId = chatId};
    }
}
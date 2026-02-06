using FluentResults;
using Microsoft.AspNetCore.Http;

namespace ManagerTask.Domain.Common.Errors;

public class ApplicationError : Error
{
    public string Code { get; }
    public int StatusCode { get; }

    public ApplicationError(string code, string message, int statusCode) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public static ApplicationError Validation(string code, string message) =>
        new ApplicationError(code, message, StatusCodes.Status400BadRequest);

    public static ApplicationError NotFound(string code, string message) =>
        new ApplicationError(code, message, StatusCodes.Status404NotFound);

    public static ApplicationError Conflict(string code, string message) =>
        new ApplicationError(code, message, StatusCodes.Status409Conflict);
}

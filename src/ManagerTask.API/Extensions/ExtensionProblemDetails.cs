using FluentResults;
using ManagerTask.Domain.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ManagerTask;

public static class ExtensionProblemDetails
{
    extension(ResultBase result)
    {
        public ActionResult<T> ToProblemDetails<T>(HttpContext context)
        {
            var firstError = result.Errors.FirstOrDefault();

            if (firstError is ApplicationError applicationError)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = $"https://api.managertask.com/errors/{applicationError.Code.Replace(".", "/")}",
                    Title = applicationError.Message,
                    Status = applicationError.StatusCode
                };

                problemDetails.Extensions["errorCode"] = applicationError.Code;
                problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

                return new ObjectResult(problemDetails)
                {
                    StatusCode = applicationError.StatusCode
                };
            }

            return new BadRequestObjectResult(new ProblemDetails
            {
                Title = firstError?.Message ?? "An error occurred.",
                Status = StatusCodes.Status400BadRequest,
            });
        }
    }
}

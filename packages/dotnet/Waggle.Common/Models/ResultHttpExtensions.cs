using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Constants;

namespace Waggle.Common.Models
{
    public static class ResultHttpExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            var response = result.ToJSend();
            var statusCode = result.Success ? StatusCodes.Status200OK : result.ErrorCode.ToStatusCode();

            return new ObjectResult(response) { StatusCode = statusCode };
        }

        public static IActionResult ToActionResult(this Result result)
        {
            var response = result.ToJSend();
            var statusCode = result.Success ? StatusCodes.Status200OK : result.ErrorCode.ToStatusCode();

            return new ObjectResult(response) { StatusCode = statusCode };
        }

        public static IActionResult ToCreatedResult<T>(this Result<T> result, string? location = null)
        {
            if (!result.Success)
                return result.ToActionResult();

            var response = result.ToJSend();
            return location != null
                ? new CreatedResult(location, response)
                : new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
        }

        public static IActionResult ToCreatedResult(this Result result, string? location = null)
        {
            if (!result.Success)
                return result.ToActionResult();

            var response = result.ToJSend();
            return location != null
                ? new CreatedResult(location, response)
                : new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
        }

        public static IActionResult ToNoContentResult(this Result result)
        {
            if (result.Success)
                return new NoContentResult();

            var response = result.ToJSend();
            var statusCode = result.ErrorCode.ToStatusCode();
            return new ObjectResult(response) { StatusCode = statusCode };
        }

        private static JSendResponse ToJSend<T>(this Result<T> result)
        {
            if (result.Success)
                return new JSendResponse {
                    Status = JSendStatus.Success,
                    Data = result.Data
                };

            var status = IsClientError(result.ErrorCode) ? JSendStatus.Fail : JSendStatus.Error;

            return new JSendResponse
            {
                Status = status,
                Message = result.Message,
                Code = result.ErrorCode,
                Data = result.ValidationErrors
            };
        }

        private static JSendResponse ToJSend(this Result result)
        {
            if (result.Success)
                return new JSendResponse
                {
                    Status = JSendStatus.Success,
                    Message = result.Message
                };

            var isClientError = IsClientError(result.ErrorCode);
            var status = isClientError ? JSendStatus.Fail : JSendStatus.Error;

            return new JSendResponse
            {
                Status = status,
                Message = result.Message,
                Code = result.ErrorCode ?? (status == JSendStatus.Error ? ErrorCodes.ServiceFailed : null),
                Data = result.ValidationErrors
            };
        }

        private static bool IsClientError(string? errorCode) => errorCode switch
        {
            ErrorCodes.Unauthorized => true,
            ErrorCodes.Forbidden => true,
            ErrorCodes.ValidationFailed => true,
            ErrorCodes.InvalidInput => true,
            ErrorCodes.NotFound => true,
            ErrorCodes.AlreadyExists => true,
            _ => false
        };

        private static int ToStatusCode(this string? errorCode) => errorCode switch
        {
            ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
            ErrorCodes.ValidationFailed => StatusCodes.Status400BadRequest,
            ErrorCodes.InvalidInput => StatusCodes.Status400BadRequest,
            ErrorCodes.NotFound => StatusCodes.Status404NotFound,
            ErrorCodes.AlreadyExists => StatusCodes.Status409Conflict,
            ErrorCodes.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
            ErrorCodes.ServiceFailed => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
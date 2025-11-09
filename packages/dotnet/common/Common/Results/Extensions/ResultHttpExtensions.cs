using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Constants;
using Waggle.Common.Models;
using Waggle.Common.Results.Core;

namespace Waggle.Common.Results.Extensions
{
    public static class ResultHttpExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            var response = result.ToJSend();
            var statusCode = result.Success ? StatusCodes.Status200OK : MapErrorCodeToStatusCode(result.ErrorCode);
            return new ObjectResult(response) { StatusCode = statusCode };
        }

        public static IActionResult ToActionResult(this Result result)
        {
            var response = result.ToJSend();
            var statusCode = result.Success ? StatusCodes.Status200OK : MapErrorCodeToStatusCode(result.ErrorCode);
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
            var statusCode = MapErrorCodeToStatusCode(result.ErrorCode);
            return new ObjectResult(response) { StatusCode = statusCode };
        }

        private static ApiResponse ToJSend<T>(this Result<T> result)
        {
            if (result.Success)
                return new ApiResponse
                {
                    Status = ApiStatus.Success,
                    Data = result.Data
                };

            return CreateFailureResponse(result.Message, result.ErrorCode, result.ValidationErrors);
        }

        private static ApiResponse ToJSend(this Result result)
        {
            if (result.Success)
                return new ApiResponse
                {
                    Status = ApiStatus.Success,
                    Data = null
                };

            return CreateFailureResponse(result.Message, result.ErrorCode, result.ValidationErrors);
        }

        private static ApiResponse CreateFailureResponse(
            string? message,
            string? errorCode,
            Dictionary<string, string[]>? validationErrors)
        {
            var status = IsClientError(errorCode) ? ApiStatus.Fail : ApiStatus.Error;
            return new ApiResponse
            {
                Status = status,
                Message = message,
                Code = errorCode ?? (status == ApiStatus.Error ? ErrorCodes.ServiceFailed : null),
                Data = validationErrors
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

        private static int MapErrorCodeToStatusCode(string? errorCode) => errorCode switch
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

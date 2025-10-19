using System.Net;
using Waggle.Common.Constants;

namespace Waggle.Common.Results
{
    public static class ResultErrorMapper
    {
        public static string FromStatusCode(HttpStatusCode code) => code switch
        {
            HttpStatusCode.BadRequest => ErrorCodes.InvalidInput,
            HttpStatusCode.Unauthorized => ErrorCodes.Unauthorized,
            HttpStatusCode.Forbidden => ErrorCodes.Forbidden,
            HttpStatusCode.NotFound => ErrorCodes.NotFound,
            HttpStatusCode.Conflict => ErrorCodes.AlreadyExists,
            HttpStatusCode.ServiceUnavailable => ErrorCodes.ServiceUnavailable,
            HttpStatusCode.InternalServerError => ErrorCodes.ServiceFailed,
            _ => ErrorCodes.ServiceFailed
        };

        public static HttpStatusCode ToStatusCode(string? errorCode) => errorCode switch
        {
            ErrorCodes.InvalidInput => HttpStatusCode.BadRequest,
            ErrorCodes.ValidationFailed => HttpStatusCode.UnprocessableEntity,
            ErrorCodes.Unauthorized => HttpStatusCode.Unauthorized,
            ErrorCodes.Forbidden => HttpStatusCode.Forbidden,
            ErrorCodes.NotFound => HttpStatusCode.NotFound,
            ErrorCodes.AlreadyExists => HttpStatusCode.Conflict,
            ErrorCodes.ServiceUnavailable => HttpStatusCode.ServiceUnavailable,
            ErrorCodes.ServiceFailed => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
    }
}

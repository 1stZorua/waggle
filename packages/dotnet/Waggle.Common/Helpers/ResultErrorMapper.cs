using System.Net;
using Waggle.Common.Constants;

namespace Waggle.Common.Helpers
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
    }
}

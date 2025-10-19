using Grpc.Core;
using Waggle.Common.Constants;

namespace Waggle.Common.Grpc
{
    public static class GrpcStatusMapper
    {
        public static string MapToErrorCode(StatusCode statusCode)
        {
            return statusCode switch
            {
                StatusCode.InvalidArgument => ErrorCodes.InvalidInput,
                StatusCode.Unauthenticated => ErrorCodes.Unauthorized,
                StatusCode.PermissionDenied => ErrorCodes.Forbidden,
                StatusCode.NotFound => ErrorCodes.NotFound,
                StatusCode.AlreadyExists => ErrorCodes.AlreadyExists,
                StatusCode.Unavailable => ErrorCodes.ServiceUnavailable,
                StatusCode.Internal => ErrorCodes.ServiceFailed,
                StatusCode.DeadlineExceeded => ErrorCodes.ServiceUnavailable,
                StatusCode.Cancelled => ErrorCodes.ServiceFailed,
                StatusCode.Aborted => ErrorCodes.ServiceFailed,
                _ => ErrorCodes.ServiceFailed
            };
        }

        public static StatusCode MapFromErrorCode(string? errorCode)
        {
            return errorCode switch
            {
                ErrorCodes.InvalidInput => StatusCode.InvalidArgument,
                ErrorCodes.ValidationFailed => StatusCode.InvalidArgument,
                ErrorCodes.Unauthorized => StatusCode.Unauthenticated,
                ErrorCodes.Forbidden => StatusCode.PermissionDenied,
                ErrorCodes.NotFound => StatusCode.NotFound,
                ErrorCodes.AlreadyExists => StatusCode.AlreadyExists,
                ErrorCodes.ServiceUnavailable => StatusCode.Unavailable,
                ErrorCodes.ServiceFailed => StatusCode.Internal,
                _ => StatusCode.Unknown
            };
        }
    }
}
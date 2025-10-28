using Grpc.Core;
using Waggle.Common.Results;

namespace Waggle.Common.Grpc
{
    public static class GrpcExceptionHelper
    {
        /// <summary>
        /// Handles RpcException from gRPC clients and converts to Result pattern.
        /// </summary>
        public static Result<T> HandleRpcException<T>(RpcException ex)
        {
            var errorCode = GrpcStatusMapper.MapToErrorCode(ex.StatusCode);

            var message = ex.StatusCode switch
            {
                StatusCode.Unavailable => "Service temporarily unavailable",
                StatusCode.DeadlineExceeded => "Request timed out",
                StatusCode.Cancelled => "Request was cancelled",
                _ => ex.Status.Detail
            };

            return Result<T>.Fail(message, errorCode);
        }

        /// <summary>
        /// Creates RpcException for gRPC servers from error code and message.
        /// </summary>
        public static RpcException CreateRpcException(string message, string? errorCode)
        {
            var statusCode = GrpcStatusMapper.MapFromErrorCode(errorCode);
            return new RpcException(new Status(statusCode, message));
        }
    }
}
using System.Net;
using Waggle.Common.Constants;
using Waggle.Common.Results.Helpers;

namespace Waggle.Common.Results.Core
{
    /// <summary>
    /// Represents the outcome of an operation that returns a value.
    /// </summary>
    /// <typeparam name="T">The type of data returned on success</typeparam>
    public class Result<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? ErrorCode { get; init; }
        public Dictionary<string, string[]>? ValidationErrors { get; init; }

        private Result(
            bool success,
            T? data = default,
            string message = "",
            string? errorCode = null,
            Dictionary<string, string[]>? validationErrors = null)
        {
            Success = success;
            Data = data;
            Message = message;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
        }

        /// <summary>Creates a successful result containing the provided data.</summary>
        public static Result<T> Ok(T data) => new(true, data);

        /// <summary>Creates a successful result with a message and data.</summary>
        public static Result<T> Ok(T data, string message) => new(true, data, message);

        /// <summary>Creates a failed result with an error message and optional error code.</summary>
        public static Result<T> Fail(string message, string? errorCode = null)
            => new(false, default, message, errorCode);

        /// <summary>Creates a failed result with validation errors for specific properties.</summary>
        public static Result<T> ValidationFail(Dictionary<string, string[]> errors, string? message = null)
            => new(false, default, message ?? "Validation failed", ErrorCodes.ValidationFailed, errors);

        /// <summary>Creates a failed result from an HTTP status code.</summary>
        public static Result<T> FromHttpStatus(HttpStatusCode code, string message)
            => Fail(message, ResultErrorMapper.FromStatusCode(code));

        /// <summary>Executes one of two functions based on success or failure.</summary>
        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
            => Success && Data is not null
                ? onSuccess(Data)
                : onFailure(Message);

        /// <summary>Transforms the data if successful.</summary>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            if (!Success || Data is null)
                return Result<TNew>.Fail(Message, ErrorCode);

            try
            {
                return Result<TNew>.Ok(mapper(Data));
            }
            catch (Exception ex)
            {
                return Result<TNew>.Fail(ex.Message);
            }
        }

        /// <summary>Asynchronously transforms the data if successful.</summary>
        public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
        {
            if (!Success || Data is null)
                return Result<TNew>.Fail(Message, ErrorCode);

            try
            {
                var result = await mapper(Data);
                return Result<TNew>.Ok(result);
            }
            catch (Exception ex)
            {
                return Result<TNew>.Fail(ex.Message);
            }
        }

        /// <summary>Performs an action on the data if successful, returning the same result.</summary>
        public Result<T> Tap(Action<T> action)
        {
            if (Success && Data is not null)
                action(Data);

            return this;
        }

        public static implicit operator Result<T>(T data) => Ok(data);
        public static implicit operator bool(Result<T> result) => result.Success;
    }

    /// <summary>
    /// Represents the outcome of an operation without a return value.
    /// </summary>
    public class Result
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? ErrorCode { get; init; }
        public Dictionary<string, string[]>? ValidationErrors { get; init; }

        private Result(
            bool success,
            string message = "",
            string? errorCode = null,
            Dictionary<string, string[]>? validationErrors = null)
        {
            Success = success;
            Message = message;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
        }

        /// <summary>Creates a successful result with optional message.</summary>
        public static Result Ok(string message = "") => new(true, message);

        /// <summary>Creates a failed result with an error message and optional error code.</summary>
        public static Result Fail(string message, string? errorCode = null)
            => new(false, message, errorCode);

        /// <summary>Creates a failed result with validation errors for specific properties.</summary>
        public static Result ValidationFail(Dictionary<string, string[]> errors, string? message = null)
            => new(false, message ?? "Validation failed", ErrorCodes.ValidationFailed, errors);

        /// <summary>Creates a failed result from an HTTP status code.</summary>
        public static Result FromHttpStatus(HttpStatusCode code, string message)
            => Fail(message, ResultErrorMapper.FromStatusCode(code));

        /// <summary>Executes one of two functions based on success or failure.</summary>
        public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)
            => Success ? onSuccess() : onFailure(Message);

        /// <summary>Converts this result into a typed result with data.</summary>
        public Result<T> ToResult<T>(T data)
            => Success ? Result<T>.Ok(data, Message) : Result<T>.Fail(Message, ErrorCode);

        public static implicit operator bool(Result result) => result.Success;
    }
}
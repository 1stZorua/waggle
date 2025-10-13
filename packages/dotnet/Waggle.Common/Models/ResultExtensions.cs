namespace Waggle.Common.Models
{
    /// <summary>
    /// Core extension methods for working with Result types.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>Converts a nullable value to a Result.</summary>
        public static Result<T> ToResult<T>(this T? value, string errorMessage = "Value is null")
            => value is not null ? Result<T>.Ok(value) : Result<T>.Fail(errorMessage);

        /// <summary>Combines multiple Results, returning the first failure or success if all succeed.</summary>
        public static Result Combine(params Result[] results)
            => results.FirstOrDefault(r => !r.Success) ?? Result.Ok();

        /// <summary>Adds a conditional validation to a Result.</summary>
        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
            => !result.Success
                ? result
                : result.Data is not null && predicate(result.Data)
                    ? result
                    : Result<T>.Fail(errorMessage);
    }
}
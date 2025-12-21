using Waggle.Common.Results.Core;

namespace Waggle.Common.Saga
{
    public interface ISagaStep<TContext, TResult>
    {
        /// <summary>
        /// Executes the main action of the step.
        /// </summary>
        Task<Result<TResult>> ExecuteAsync(TContext context);

        /// <summary>
        /// Executes a compensation if the step fails.
        /// </summary>
        Task<Result<TResult>> CompensateAsync(TContext context);
    }

    public interface ISagaStep<TContext>
    {
        /// <summary>
        /// Executes the main action of the step.
        /// </summary>
        Task<Result> ExecuteAsync(TContext context);

        /// <summary>
        /// Executes a compensation if the step fails.
        /// </summary>
        Task<Result> CompensateAsync(TContext context);
    }
}

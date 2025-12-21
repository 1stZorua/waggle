using Waggle.Common.Results.Core;

namespace Waggle.Common.Saga
{
    public interface ISagaCoordinator<TContext, TResult>
    {
        Task<Result<TResult>> ExecuteAsync(TContext context);
        void AddStep(ISagaStep<TContext, TResult> step);
    }

    public interface ISagaCoordinator<TContext>
    {
        Task<Result> ExecuteAsync(TContext context);
        void AddStep(ISagaStep<TContext> step);
    }

}

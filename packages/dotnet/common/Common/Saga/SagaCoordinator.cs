using Waggle.Common.Results.Core;

namespace Waggle.Common.Saga
{
    public class SagaCoordinator<TContext, TResult> : ISagaCoordinator<TContext, TResult>
    {
        private readonly List<ISagaStep<TContext, TResult>> _steps = [];

        public void AddStep(ISagaStep<TContext, TResult> step) => _steps.Add(step);

        public async Task<Result<TResult>> ExecuteAsync(TContext context)
        {
            var executedSteps = new Stack<ISagaStep<TContext, TResult>>();

            int stepIndex = 0;
            int lastIndex = _steps.Count - 1;

            foreach (var step in _steps)
            {
                if (stepIndex == lastIndex) break;

                var result = await step.ExecuteAsync(context);

                if (!result.Success)
                {
                    await CompensateAsync(executedSteps, context);
                    return result;
                }

                executedSteps.Push(step);
                stepIndex++;
            }

            return await _steps[lastIndex].ExecuteAsync(context);
        }

        private static async Task CompensateAsync(Stack<ISagaStep<TContext, TResult>> steps, TContext context)
        {
            while (steps.Count > 0)
            {
                var step = steps.Pop();
                await step.CompensateAsync(context);
            }
        }
    }

    public class SagaCoordinator<TContext> : ISagaCoordinator<TContext>
    {
        private readonly List<ISagaStep<TContext>> _steps = [];

        public void AddStep(ISagaStep<TContext> step) => _steps.Add(step);

        public async Task<Result> ExecuteAsync(TContext context)
        {
            var executedSteps = new Stack<ISagaStep<TContext>>();
            int stepIndex = 0;
            int lastIndex = _steps.Count - 1;

            foreach (var step in _steps)
            {
                if (stepIndex == lastIndex) break;

                var result = await step.ExecuteAsync(context);
                if (!result.Success)
                {
                    await CompensateAsync(executedSteps, context);
                    return result;
                }

                executedSteps.Push(step);
                stepIndex++;
            }

            return await _steps[lastIndex].ExecuteAsync(context);
        }

        private static async Task CompensateAsync(Stack<ISagaStep<TContext>> steps, TContext context)
        {
            while (steps.Count > 0)
            {
                var step = steps.Pop();
                await step.CompensateAsync(context);
            }
        }
    }
}

using Waggle.Common.Results.Core;

namespace Waggle.Common.Validation
{
    public interface IServiceValidator
    {
        Task<Result> ValidateAsync<T>(T dto);
    }
}

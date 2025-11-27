using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Waggle.Common.Results.Core;

namespace Waggle.Common.Validation
{
    public class ServiceValidator : IServiceValidator
    {
        private readonly IServiceProvider _provider;

        public ServiceValidator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<Result> ValidateAsync<T>(T dto)
        {
            var validator = _provider.GetService<IValidator<T>>();
            if (validator == null) return Result.Ok();

            var validationResult = await validator.ValidateAsync(dto);

            if (validationResult.IsValid) return Result.Ok();

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return Result.ValidationFail(errors);
        }
    }
}

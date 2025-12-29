using FluentValidation;
using Waggle.FavoriteService.Dtos;
using Waggle.FavoriteService.Constants;

namespace Waggle.FavoriteService.Validators
{
    internal sealed class FavoriteCreateDtoValidator : AbstractValidator<FavoriteCreateDto>
    {
        public FavoriteCreateDtoValidator()
        {
            #region Target
            RuleFor(x => x.TargetId)
                .NotEmpty()
                .WithMessage(FavoriteValidationErrors.Create.TargetRequired);

            RuleFor(x => x.TargetType)
                .IsInEnum()
                .WithMessage(FavoriteValidationErrors.Create.TargetTypeInvalid);
            #endregion
        }
    }
}

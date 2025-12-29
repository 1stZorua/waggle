using FluentValidation;
using Waggle.Common.Models;
using Waggle.FavoriteService.Constants;

namespace Waggle.FavoriteService.Validators
{
    internal sealed class FavoriteTargetTypeValidator : AbstractValidator<InteractionType>
    {
        public FavoriteTargetTypeValidator()
        {
            #region Target
            RuleFor(x => x)
                .IsInEnum()
                .WithMessage(FavoriteValidationErrors.Create.TargetTypeInvalid);
            #endregion
        }
    }
}

using FluentValidation;
using Waggle.Common.Models;
using Waggle.LikeService.Constants;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Validators
{
    internal sealed class LikeTargetTypeValidator : AbstractValidator<InteractionType>
    {
        public LikeTargetTypeValidator()
        {
            #region Target
            RuleFor(x => x)
                .IsInEnum()
                .WithMessage(LikeValidationErrors.Create.TargetTypeInvalid);
            #endregion
        }
    }
}

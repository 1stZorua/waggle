using FluentValidation;
using Waggle.LikeService.Dtos;
using Waggle.LikeService.Constants;

namespace Waggle.LikeService.Validators
{
    internal sealed class LikeCreateDtoValidator : AbstractValidator<LikeCreateDto>
    {
        public LikeCreateDtoValidator()
        {
            #region Target
            RuleFor(x => x.TargetId)
                .NotEmpty()
                .WithMessage(LikeValidationErrors.Create.TargetRequired);

            RuleFor(x => x.TargetType)
                .IsInEnum()
                .WithMessage(LikeValidationErrors.Create.TargetTypeInvalid);
            #endregion
        }
    }
}

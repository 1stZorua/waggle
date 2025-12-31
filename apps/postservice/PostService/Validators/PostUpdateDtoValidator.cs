using FluentValidation;
using Waggle.PostService.Constants;
using Waggle.PostService.Dtos;

namespace Waggle.PostService.Validators
{
    internal sealed class PostUpdateDtoValidator : AbstractValidator<PostUpdateDto>
    {
        public PostUpdateDtoValidator()
        {
            #region Caption
            RuleFor(x => x.Caption)
                .NotEmpty()
                .WithMessage(PostValidationErrors.Create.CaptionRequired)
                .MaximumLength(1000)
                .WithMessage(string.Format(PostValidationErrors.Create.CaptionTooLong, 1000));
            #endregion
        }
    }
}

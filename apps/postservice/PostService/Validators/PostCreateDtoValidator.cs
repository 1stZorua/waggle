using FluentValidation;
using Waggle.PostService.Constants;
using Waggle.PostService.Dtos;

namespace Waggle.PostService.Validators
{
    internal sealed class PostCreateDtoValidator : AbstractValidator<PostCreateDto>
    {
        public PostCreateDtoValidator()
        {
            #region Caption
            RuleFor(x => x.Caption)
                .NotEmpty()
                .WithMessage(PostValidationErrors.Create.CaptionRequired)
                .MaximumLength(1000)
                .WithMessage(string.Format(PostValidationErrors.Create.CaptionTooLong, 1000));
            #endregion

            #region MediaIds
            RuleFor(x => x.MediaIds)
                .NotNull()
                .WithMessage(PostValidationErrors.Create.MediaRequired)
                .Must(ids => ids.Count >= 1 && ids.Count <= 4)
                .WithMessage(PostValidationErrors.Create.MediaCountInvalid);
            #endregion
        }
    }
}

using FluentValidation;
using Waggle.CommentService.Constants;
using Waggle.CommentService.Dtos;

namespace Waggle.CommentService.Validators
{
    public class CommentCreateDtoValidator: AbstractValidator<CommentCreateDto>
    {
        public CommentCreateDtoValidator() 
        {
            #region Content
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage(CommentValidationErrors.Create.ContentRequired)
                .MaximumLength(500)
                .WithMessage(string.Format(CommentValidationErrors.Create.ContentTooLong, 500));
            #endregion
        }
    }
}

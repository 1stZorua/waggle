using FluentValidation;
using Waggle.UserService.Constants;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Validators
{
    internal sealed class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            #region AvatarId
            RuleFor(x => x.AvatarId)
                .NotEmpty()
                .WithMessage(UserValidationErrors.Update.AvatarRequired);
            #endregion
        }
    }
}
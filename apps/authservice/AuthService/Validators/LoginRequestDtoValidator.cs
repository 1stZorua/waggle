using FluentValidation;
using Microsoft.Extensions.Options;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Options;

namespace Waggle.AuthService.Validators
{
    internal sealed class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator(IOptions<AuthOptions> options)
        {
            var authOptions = options.Value;

            #region Identifier

            RuleFor(x => x.Identifier)
                .NotEmpty()
                    .WithMessage(AuthValidationErrors.Login.IdentifierRequired)
                .EmailAddress()
                    .When(x => x.Identifier.Contains('@'))
                    .WithMessage(AuthValidationErrors.Login.EmailInvalid);

            #endregion

            #region Password

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage(AuthValidationErrors.Login.PasswordRequired);

            #endregion
        }
    }
}

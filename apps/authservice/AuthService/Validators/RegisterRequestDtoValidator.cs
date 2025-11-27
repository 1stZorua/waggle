using FluentValidation;
using Microsoft.Extensions.Options;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Options;
using Waggle.Common.Validation;

namespace Waggle.AuthService.Validators
{
    internal sealed class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator(IOptions<AuthOptions> options)
        {
            var authOptions = options.Value;

            #region Username
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(AuthValidationErrors.Register.UsernameRequired)
                .Length(authOptions.MinUsernameLength, authOptions.MaxUsernameLength)
                    .WithMessage(string.Format(
                        AuthValidationErrors.Register.UsernameInvalid,
                        authOptions.MinUsernameLength,
                        authOptions.MaxUsernameLength))
                .Matches(ValidationPatterns.Alphanumeric)
                    .WithMessage(AuthValidationErrors.Register.UsernameInvalid);
            #endregion

            #region Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(AuthValidationErrors.Register.EmailRequired)
                .EmailAddress().WithMessage(AuthValidationErrors.Register.EmailInvalid);
            #endregion

            #region Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(AuthValidationErrors.Register.PasswordRequired)
                .MinimumLength(authOptions.MinPasswordLength)
                    .WithMessage(string.Format(
                        AuthValidationErrors.Register.PasswordTooShort,
                        authOptions.MinPasswordLength))
                .MaximumLength(authOptions.MaxPasswordLength)
                    .WithMessage(string.Format(
                        AuthValidationErrors.Register.PasswordTooLong,
                        authOptions.MaxPasswordLength));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(AuthValidationErrors.Register.ConfirmPasswordRequired)
                .Equal(x => x.Password).WithMessage(AuthValidationErrors.Register.ConfirmPasswordMismatch);
            #endregion

            #region Names
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(AuthValidationErrors.Register.FirstNameRequired)
                .Matches(ValidationPatterns.LettersOnly)
                    .WithMessage(AuthValidationErrors.Register.FirstNameInvalid);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(AuthValidationErrors.Register.LastNameRequired)
                .Matches(ValidationPatterns.LettersOnly)
                    .WithMessage(AuthValidationErrors.Register.LastNameInvalid);
            #endregion
        }
    }
}

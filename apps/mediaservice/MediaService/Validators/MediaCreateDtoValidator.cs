using FluentValidation;
using Waggle.Common.Validation;
using Waggle.MediaService.Constants;
using Waggle.MediaService.Dtos;

namespace Waggle.MediaService.Validators
{
    internal sealed class MediaCreateDtoValidator : AbstractValidator<MediaCreateDto>
    {
        public MediaCreateDtoValidator()
        {
            #region File
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage(MediaValidationErrors.Upload.FileRequired);

            RuleFor(x => x.File.ContentType)
                .Must(ct => ImageTypes.ContentTypes.Contains(ct.ToLower()))
                .WithMessage(string.Format(
                    MediaValidationErrors.Upload.InvalidFileType,
                    ImageTypes.DisplayNames))
                .When(x => x.File != null);

            RuleFor(x => x.File.Length)
                .GreaterThan(0)
                .WithMessage(MediaValidationErrors.Upload.FileEmpty)
                .LessThanOrEqualTo(ImageTypes.MaxFileSize)
                .WithMessage(string.Format(
                    MediaValidationErrors.Upload.FileTooLarge,
                    ImageTypes.MaxFileSize / (1024 * 1024)))
                .When(x => x.File != null);

            RuleFor(x => x.File.FileName)
                .Must(fileName =>
                {
                    if (string.IsNullOrEmpty(fileName)) return false;
                    var ext = Path.GetExtension(fileName)?.ToLower();
                    return !string.IsNullOrEmpty(ext) && ImageTypes.Extensions.Contains(ext);
                })
                .WithMessage(string.Format(
                    MediaValidationErrors.Upload.InvalidFileExtension,
                    string.Join(", ", ImageTypes.Extensions)))
                .When(x => x.File != null);
            #endregion

            #region BucketName
            RuleFor(x => x.BucketName)
                .NotEmpty()
                .WithMessage(MediaValidationErrors.Upload.BucketNameRequired)
                .Matches(ValidationPatterns.LowercaseLettersOnly)
                .WithMessage(MediaValidationErrors.Upload.BucketNameInvalid);
            #endregion

            #region Prefix (Optional)
            RuleFor(x => x.Prefix)
                .MaximumLength(255)
                .WithMessage(string.Format(
                    MediaValidationErrors.Upload.PrefixTooLong,
                    255))
                .When(x => !string.IsNullOrEmpty(x.Prefix));
            #endregion
        }
    }
}

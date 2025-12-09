namespace Waggle.MediaService.Constants
{
    public static class MediaValidationErrors
    {
        public static class Upload
        {
            public const string FileRequired = "File is required";
            public const string FileEmpty = "File cannot be empty";
            public const string FileTooLarge = "File size must not exceed {0}MB";
            public const string InvalidFileType = "Only {0} images are allowed";
            public const string InvalidFileExtension = "File must have one of the following extensions: {0}";

            public const string BucketNameRequired = "Bucket name is required";
            public const string BucketNameInvalid = "Bucket name must contain only lowercase letters";

            public const string PrefixTooLong = "Prefix must not exceed {0} characters";
        }
    }
}

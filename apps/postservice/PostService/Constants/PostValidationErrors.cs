namespace Waggle.PostService.Constants
{
    public static class PostValidationErrors
    {
        public static class Create
        {
            public const string CaptionRequired = "Caption is required";
            public const string CaptionTooLong = "Caption cannot exceed {0} characters";

            public const string ThumbnailRequired = "Thumbnail is required";

            public const string MediaRequired = "At least one media item is required";
            public const string MediaCountInvalid = "You can attach between 1 and 4 media items";
        }
    }
}

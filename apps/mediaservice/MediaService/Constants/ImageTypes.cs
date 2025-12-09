namespace Waggle.MediaService.Constants
{
    public static class ImageTypes
    {
        public static readonly string[] ContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/avif"
        ];

        public static readonly string[] Extensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".avif",
        ];

        public const long MaxFileSize = 10 * 1024 * 1024;

        public const string DisplayNames = "JPEG, PNG, WebP, and AVIF";
    }
}

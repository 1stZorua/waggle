namespace Waggle.CommentService.Constants
{
    public static class CommentValidationErrors
    {
        public static class Create
        {
            public const string ContentRequired = "Content is required";
            public const string ContentTooLong = "Content cannot exceed {0} characters";
        }
    }
}

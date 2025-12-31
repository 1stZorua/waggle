using Waggle.Common.Auth;

namespace Waggle.CommentService.Saga.Context
{
    public class DeletionSagaContext
    {
        public required Guid Id { get; set; }
        public required UserInfoDto CurrentUser { get; set; }
        public List<Guid> DeletedCommentIds { get; set; } = [];
    }
}

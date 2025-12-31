using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Comment.Events;
using Waggle.Contracts.Post.Events;

namespace Waggle.FollowService.Services
{
    public interface ILikeEventHandler
    {
        Task<Result> HandleCommentDeletedEventAsync(CommentDeletedEvent @event);
        Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event);
        Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event);
    }
}

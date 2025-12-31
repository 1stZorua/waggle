using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Post.Events;

namespace Waggle.CommentService.Services
{
    public interface ICommentEventHandler
    {
        Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event);
        Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event);
    }
}

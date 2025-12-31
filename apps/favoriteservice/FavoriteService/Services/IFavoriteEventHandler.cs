using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Post.Events;

namespace Waggle.FavoriteService.Services
{
    public interface IFavoriteEventHandler
    {
        Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event);
        Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event);
    }
}

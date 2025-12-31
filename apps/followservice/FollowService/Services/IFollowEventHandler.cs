using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;

namespace Waggle.FollowService.Services
{
    public interface IFollowEventHandler
    {
        Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event);
    }
}

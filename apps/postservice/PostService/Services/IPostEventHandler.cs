using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;

namespace Waggle.PostService.Services
{
    public interface IPostEventHandler
    {
        Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event);
    }
}

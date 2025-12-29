using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Services
{
    public interface IUserEventHandler
    {
        Task<Result<UserDto>> HandleUserRegisteredEventAsync(RegisteredEvent @event);
        Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event);
    }
}

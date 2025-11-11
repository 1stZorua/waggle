using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Services
{
    public interface IUserEventHandler
    {
        Task<Result<UserDto>> HandleUserRegisteredAsync(RegisteredEvent @event);
        Task<Result> HandleUserDeletedAsync(DeletedEvent @event);
    }
}

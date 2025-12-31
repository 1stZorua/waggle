using AutoMapper;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.User.Events;
using Waggle.UserService.Dtos;
using Waggle.UserService.Models;

namespace Waggle.UserService.Profiles
{
    public class UsersEventProfile : Profile
    {
        public UsersEventProfile()
        {
            CreateMap<User, UserUpdatedEvent>();
            CreateMap<RegisteredEvent, UserCreateDto>();
        }
    }
}

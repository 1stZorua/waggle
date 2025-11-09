using AutoMapper;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<Models.User, UserDto>();
            CreateMap<UserCreateDto, Models.User>();
        }
    }
}

using AutoMapper;
using UserService.Dtos;

namespace UserService.Profiles
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

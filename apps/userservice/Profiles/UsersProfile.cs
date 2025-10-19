using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using UserService.Dtos;
using UserService.Grpc;

namespace UserService.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<Models.User, UserDto>();
            CreateMap<UserCreateDto, Models.User>();

            CreateMap<CreateUserRequest, UserCreateDto>();
            CreateMap<GetUserByIdRequest, Guid>();

            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());
        }
    }
}

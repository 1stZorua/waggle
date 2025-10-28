using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using UserService.Dtos;
using Waggle.Contracts.User.Grpc;

namespace UserService.Profiles
{
    public class UsersGrpcProfile : Profile
    {
        public UsersGrpcProfile()
        {

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

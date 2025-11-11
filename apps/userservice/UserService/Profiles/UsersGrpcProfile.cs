using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.Contracts.User.Grpc;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Profiles
{
    public class UsersGrpcProfile : Profile
    {
        public UsersGrpcProfile()
        {

            CreateMap<CreateUserRequest, UserCreateDto>();
            CreateMap<GetUserByIdRequest, Guid>();
            CreateMap<DeleteUserRequest, Guid>();

            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());

            CreateMap<Common.Pagination.Models.PageInfo, PageInfo>();
        }
    }
}

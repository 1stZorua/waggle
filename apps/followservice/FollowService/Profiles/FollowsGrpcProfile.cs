using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.Contracts.Follow.Grpc;
using Waggle.FollowService.Dtos;

namespace Waggle.FollowService.Profiles
{
    public class FollowsGrpcProfile : Profile
    {
        public FollowsGrpcProfile()
        {
            CreateMap<CreateFollowRequest, FollowCreateDto>();
            CreateMap<GetFollowByIdRequest, Guid>();
            CreateMap<DeleteFollowRequest, Guid>();

            CreateMap<FollowDto, CreateFollowResponse>();
            CreateMap<FollowDto, GetFollowByIdResponse>();
            CreateMap<FollowDto, IsFollowingResponse>();
            CreateMap<FollowDto, Follow>();
            CreateMap<Follow, FollowDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));
            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());

            CreateMap<Common.Pagination.Models.PageInfo, PageInfo>();
        }
    }
}
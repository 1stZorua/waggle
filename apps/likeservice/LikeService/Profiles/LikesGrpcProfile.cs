using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.Contracts.Like.Grpc;
using Waggle.LikeService.Dtos;

namespace Waggle.LikeService.Profiles
{
    public class LikesGrpcProfile : Profile
    {
        public LikesGrpcProfile()
        {
            CreateMap<CreateLikeRequest, LikeCreateDto>();
            CreateMap<GetLikeByIdRequest, Guid>();
            CreateMap<DeleteLikeRequest, Guid>();

            CreateMap<LikeDto, CreateLikeResponse>();
            CreateMap<LikeDto, GetLikeByIdResponse>();
            CreateMap<LikeDto, HasLikedResponse>();
            CreateMap<LikeDto, Like>();
            CreateMap<Like, LikeDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());

            CreateMap<Common.Pagination.Models.PageInfo, PageInfo>();
            CreateMap<InteractionType, Common.Models.InteractionType>();
            CreateMap<Common.Models.InteractionType, InteractionType>();
        }
    }
}

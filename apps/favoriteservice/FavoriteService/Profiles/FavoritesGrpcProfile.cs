using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.Contracts.Favorite.Grpc;
using Waggle.FavoriteService.Dtos;

namespace Waggle.FavoriteService.Profiles
{
    public class FavoritesGrpcProfile : Profile
    {
        public FavoritesGrpcProfile()
        {
            CreateMap<CreateFavoriteRequest, FavoriteCreateDto>();
            CreateMap<GetFavoriteByIdRequest, Guid>();
            CreateMap<DeleteFavoriteRequest, Guid>();

            CreateMap<FavoriteDto, CreateFavoriteResponse>();
            CreateMap<FavoriteDto, GetFavoriteByIdResponse>();
            CreateMap<FavoriteDto, HasFavoritedResponse>();
            CreateMap<FavoriteDto, Favorite>();
            CreateMap<Favorite, FavoriteDto>();

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

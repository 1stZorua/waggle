using AutoMapper;
using Waggle.FavoriteService.Dtos;

namespace Waggle.FavoriteService.Profiles
{
    public class FavoritesProfile : Profile
    {
        public FavoritesProfile()
        {
            CreateMap<Models.Favorite, FavoriteDto>();
            CreateMap<FavoriteCreateDto, Models.Favorite>();
        }
    }
}

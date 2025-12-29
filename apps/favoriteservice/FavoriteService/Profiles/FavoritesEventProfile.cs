using AutoMapper;
using Waggle.Contracts.Favorite.Events;
using Waggle.FavoriteService.Models;

namespace Waggle.FavoriteService.Profiles
{
    public class FavoritesEventProfile : Profile
    {
        public FavoritesEventProfile()
        {
            CreateMap<Favorite, FavoriteCreatedEvent>();
            CreateMap<Favorite, FavoriteDeletedEvent>();
        }
    }
}

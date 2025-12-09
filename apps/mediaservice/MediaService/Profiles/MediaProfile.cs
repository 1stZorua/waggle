using AutoMapper;
using Waggle.MediaService.Dtos;
using Waggle.MediaService.Models;

namespace Waggle.MediaService.Profiles
{
    public class MediaProfile : Profile
    {
        public MediaProfile()
        {
            CreateMap<Media, MediaDto>();
            CreateMap<MediaDto, Media>();
            CreateMap<MediaCreateDto, Media>()
                .ForMember(dest => dest.ObjectName, opt => opt.Ignore());
        }
    }
}

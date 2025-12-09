using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.Contracts.Media.Grpc;
using Waggle.MediaService.Dtos;

namespace Waggle.MediaService.Profiles
{
    public class MediaGrpcProfile : Profile
    {
        public MediaGrpcProfile()
        {
            CreateMap<UploadMediaRequest, MediaCreateDto>();
            CreateMap<GetMediaByIdRequest, Guid>();
            CreateMap<GetMediaByIdsRequest, MediaBatchRequest>();
            CreateMap<GetMediaUrlRequest, Guid>();
            CreateMap<GetMediaUrlsRequest, MediaBatchRequest>();
            CreateMap<DeleteMediaRequest, Guid>();

            CreateMap<UrlResponseDto, GetMediaUrlResponse>();
            CreateMap<Dictionary<Guid, UrlResponseDto>, GetMediaUrlsResponse>();
            CreateMap<GetMediaUrlResponse, UrlResponseDto>();
            CreateMap<GetMediaUrlsResponse, Dictionary<Guid, UrlResponseDto>>();
            CreateMap<MediaDto, GetMediaByIdResponse>();
            CreateMap<MediaDto, UploadMediaResponse>();
            CreateMap<MediaDto, Media>();
            CreateMap<Media, MediaDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());

            CreateMap<Common.Pagination.Models.PageInfo, PageInfo>();
        }
    }
}

using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.Contracts.Post.Grpc;
using Waggle.PostService.Dtos;

namespace Waggle.PostService.Profiles
{
    public class PostsGrpcProfile : Profile
    {
        public PostsGrpcProfile()
        {
            CreateMap<CreatePostRequest, PostCreateDto>();
            CreateMap<GetPostByIdRequest, Guid>();
            CreateMap<DeletePostRequest, Guid>();

            CreateMap<PostDto, CreatePostResponse>();
            CreateMap<PostDto, GetPostByIdResponse>();
            CreateMap<PostDto, Post>();
            CreateMap<Post, PostDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());

            CreateMap<Common.Pagination.Models.PageInfo, PageInfo>();
        }
    }
}

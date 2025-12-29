using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Waggle.CommentService.Dtos;
using Waggle.Contracts.Comment.Grpc;

namespace Waggle.CommentService.Profiles
{
    public class CommentsGrpcProfile : Profile
    {
        public CommentsGrpcProfile()
        {
            CreateMap<CreateCommentRequest, CommentCreateDto>();
            CreateMap<UpdateCommentRequest, CommentUpdateDto>();
            CreateMap<GetCommentByIdRequest, Guid>();
            CreateMap<DeleteCommentRequest, Guid>();

            CreateMap<CommentDto, CreateCommentResponse>();
            CreateMap<CommentDto, GetCommentByIdResponse>();
            CreateMap<CommentDto, Comment>();
            CreateMap<Comment, CommentDto>();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(dt => Timestamp.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(ts => ts.ToDateTime());

            CreateMap<Common.Pagination.Models.PageInfo, PageInfo>();
        }
    }
}

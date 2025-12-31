using AutoMapper;
using Waggle.CommentService.Models;
using Waggle.CommentService.Saga.Context;
using Waggle.Contracts.Comment.Events;

namespace Waggle.CommentService.Profiles
{
    public class CommentsEventProfile : Profile
    {
        public CommentsEventProfile()
        {
            CreateMap<Comment, CommentCreatedEvent>();
            CreateMap<Comment, CommentUpdatedEvent>();
            CreateMap<Comment, CommentDeletedEvent>();
            CreateMap<DeletionSagaContext, CommentDeletedEvent>();
        }
    }
}

using AutoMapper;
using Waggle.Contracts.Post.Events;
using Waggle.PostService.Models;
using Waggle.PostService.Saga.Context;

namespace Waggle.PostService.Profiles
{
    public class PostsEventProfile : Profile
    {
        public PostsEventProfile() 
        {
            CreateMap<Post, PostCreatedEvent>();
            CreateMap<Post, PostUpdatedEvent>();
            CreateMap<Post, PostDeletedEvent>();
            CreateMap<DeletionSagaContext, PostDeletedEvent>();
        }
    }
}

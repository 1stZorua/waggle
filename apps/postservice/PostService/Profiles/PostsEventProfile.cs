using AutoMapper;
using Waggle.Contracts.Post.Events;
using Waggle.PostService.Saga.Context;

namespace Waggle.PostService.Profiles
{
    public class PostsEventProfile : Profile
    {
        public PostsEventProfile() 
        {
            CreateMap<DeletionSagaContext, PostDeletedEvent>();
        }
    }
}

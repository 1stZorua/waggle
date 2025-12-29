using Waggle.Common.Auth;

namespace Waggle.PostService.Saga.Context
{
    public class DeletionSagaContext
    {
        public required Guid Id { get; set; }
        public required UserInfoDto CurrentUser { get; set; }
    }
}

namespace Waggle.UserService.Dtos
{
    public class UserBatchRequest
    {
        public required IEnumerable<Guid> Ids { get; set; }
    }
}

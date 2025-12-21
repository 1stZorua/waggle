namespace Waggle.AuthService.Saga.Context
{
    public class RegistrationSagaContext
    {
        public Guid Id { get; set; }
        public required string Username { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Password { get; init; }
    }
}

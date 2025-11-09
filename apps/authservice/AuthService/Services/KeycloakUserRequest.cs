namespace Waggle.AuthService.Services
{
    public record KeycloakUserRequest(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password,
        bool Enabled = true,
        Credential[] Credentials = null!
    );

    public record Credential(
        string Type,
        string Value,
        bool Temporary
    );
}

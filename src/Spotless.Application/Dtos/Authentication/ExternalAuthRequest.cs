namespace Spotless.Application.Dtos.Authentication
{
    public record ExternalAuthRequest(
        string Provider,
        string IdToken
    );
}

namespace Spotless.Application.Dtos.Driver
{
    public record ApproveDriverRequest
    {
        public string Password { get; init; } = string.Empty;
    }
}
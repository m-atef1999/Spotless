namespace Spotless.Application.Interfaces
{
    public interface IAuthUser
    {
        Guid Id { get; }
        string? Email { get; }
        string? UserName { get; }
        bool EmailConfirmed { get; }

    }
}

namespace Spotless.Application.Interfaces
{
    public interface IAuthUser
    {
        Guid Id { get; }
        string? Email { get; }
        string? UserName { get; }
        string Name { get; }
        bool EmailConfirmed { get; }
        Guid? CustomerId { get; }

    }
}

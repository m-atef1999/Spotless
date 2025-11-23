using System.Security.Claims;

namespace Spotless.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        Guid? CustomerId { get; }
        bool IsAuthenticated { get; }
        ClaimsPrincipal? User { get; }
    }
}

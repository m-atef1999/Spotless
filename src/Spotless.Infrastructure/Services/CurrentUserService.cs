using Microsoft.AspNetCore.Http;
using Spotless.Application.Interfaces;
using System.Security.Claims;

namespace Spotless.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public Guid? CustomerId 
        {
            get 
            {
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("CustomerId");
                return claim != null && Guid.TryParse(claim.Value, out var id) ? id : null;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    }
}

using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.RejectDriverRegistration
{
    public record RejectDriverRegistrationCommand(
        Guid ApplicationId,
        Guid AdminId,
        string Reason
    ) : IRequest<Unit>, Interfaces.ICacheInvalidator
    {
        public string[] CacheKeys => ["drivers:all:v4"];
    }
}

using MediatR;

namespace Spotless.Application.Features.Orders.Commands.RejectOrderApplication
{
    public record RejectOrderApplicationCommand(Guid ApplicationId, Guid AdminId) : IRequest<Unit>;
}

using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.RevokeDriverAccess
{
    public record RevokeDriverAccessCommand(Guid DriverId) : IRequest;
}

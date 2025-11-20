using MediatR;

namespace Spotless.Application.Features.Orders.Commands.ApplyToOrder
{
    public record ApplyToOrderCommand(Guid OrderId, Guid DriverId) : IRequest<Guid>;
}

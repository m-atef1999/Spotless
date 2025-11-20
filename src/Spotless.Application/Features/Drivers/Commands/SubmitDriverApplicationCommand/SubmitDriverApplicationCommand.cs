using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand
{
    public record SubmitDriverApplicationCommand(
        Guid CustomerId,
        DriverApplicationRequest Dto
    ) : IRequest<Guid>;
}

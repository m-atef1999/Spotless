using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands
{
    public record SubmitDriverApplicationCommand(
        Guid CustomerId,
        SubmitDriverApplicationDto Dto
    ) : IRequest<Guid>;
}
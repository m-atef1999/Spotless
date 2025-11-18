using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand
{
    public record SubmitDriverApplicationCommand(
        Guid CustomerId,
        SubmitDriverApplicationDto Dto
    ) : IRequest<Guid>;
}
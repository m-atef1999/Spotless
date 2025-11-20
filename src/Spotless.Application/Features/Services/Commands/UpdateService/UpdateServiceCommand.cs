using MediatR;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services.Commands.UpdateService
{

    public record UpdateServiceCommand(
        UpdateServiceDto Dto,
        Guid AdminId
    ) : IRequest<Unit>;
}

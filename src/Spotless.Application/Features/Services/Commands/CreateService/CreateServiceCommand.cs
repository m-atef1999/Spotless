using MediatR;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services.Commands.CreateService
{

    public record CreateServiceCommand(
        CreateServiceDto Dto,
        Guid AdminId
    ) : IRequest<Guid>;
}

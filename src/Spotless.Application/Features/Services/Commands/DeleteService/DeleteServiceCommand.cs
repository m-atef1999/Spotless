using MediatR;

namespace Spotless.Application.Features.Services.Commands.DeleteService
{
    public record DeleteServiceCommand(Guid ServiceId) : IRequest<Unit>;
}

using MediatR;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services
{
    public record GetServicesByCategoryQuery(
        Guid CategoryId) : IRequest<IReadOnlyList<ServiceDto>>;
}

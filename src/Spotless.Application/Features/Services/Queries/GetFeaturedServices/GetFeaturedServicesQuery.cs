using MediatR;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services.Queries.GetFeaturedServices
{
    public record GetFeaturedServicesQuery(int Count = 4) : IRequest<IReadOnlyList<ServiceDto>>;
}

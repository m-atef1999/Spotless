using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Services.Queries.GetFeaturedServices
{
    public record GetFeaturedServicesQuery(int Count = 4) : IQuery<IReadOnlyList<ServiceDto>>;
}

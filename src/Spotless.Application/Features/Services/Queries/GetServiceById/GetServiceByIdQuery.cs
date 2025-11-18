using Spotless.Application.Interfaces;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services.Queries.GetServiceById
{
    public record GetServiceByIdQuery(Guid ServiceId) : IQuery<ServiceDto>;
}

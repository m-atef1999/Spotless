using Spotless.Application.Dtos.Service;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public interface IServiceMapper
    {
        ServiceDto MapToDto(Service entity);
        IEnumerable<ServiceDto> MapToDto(IEnumerable<Service> entities);
    }
}

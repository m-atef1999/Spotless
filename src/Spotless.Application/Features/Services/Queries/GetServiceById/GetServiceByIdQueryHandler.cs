using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services.Queries.GetServiceById
{
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceMapper _serviceMapper;

        public GetServiceByIdQueryHandler(IServiceRepository serviceRepository, IServiceMapper serviceMapper)
        {
            _serviceRepository = serviceRepository;
            _serviceMapper = serviceMapper;
        }

        public async Task<ServiceDto> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
            {
                throw new KeyNotFoundException($"Service with ID {request.ServiceId} not found.");
            }

            return _serviceMapper.MapToDto(service);
        }
    }
}

using MediatR;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Services
{
    public record ListAllServicesQuery() : IRequest<IReadOnlyList<ServiceDto>>;

    public class ListAllServicesQueryHandler : IRequestHandler<ListAllServicesQuery, IReadOnlyList<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListAllServicesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(ListAllServicesQuery request, CancellationToken cancellationToken)
        {

            var services = await _unitOfWork.Services.GetAllAsync();

            if (services == null || !services.Any())
            {
                return new List<ServiceDto>();
            }


            return services.Select(s => s.ToDto()).ToList();
        }
    }
}

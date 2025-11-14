using MediatR;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Services.Queries.GetServicesByCategory
{
    public class GetServicesByCategoryQueryHandler : IRequestHandler<GetServicesByCategoryQuery, IReadOnlyList<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetServicesByCategoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(GetServicesByCategoryQuery request, CancellationToken cancellationToken)
        {
            var services = await _unitOfWork.Services.GetAsync(s => s.CategoryId == request.CategoryId);

            if (services == null || !services.Any())
            {
                return new List<ServiceDto>();
            }

            return services.Select(s => s.ToDto()).ToList();
        }
    }
}

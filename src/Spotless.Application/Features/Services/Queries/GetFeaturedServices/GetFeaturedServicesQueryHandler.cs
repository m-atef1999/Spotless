using MediatR;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Services.Queries.GetFeaturedServices
{
    public class GetFeaturedServicesQueryHandler : IRequestHandler<GetFeaturedServicesQuery, IReadOnlyList<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFeaturedServicesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(GetFeaturedServicesQuery request, CancellationToken cancellationToken)
        {

            var allServices = await _unitOfWork.Services.GetAllAsync();

            if (allServices == null || !allServices.Any())
            {
                return new List<ServiceDto>();
            }


            var featuredServices = allServices
                .Take(request.Count)
                .ToList();

            return featuredServices.Select(s => s.ToDto()).ToList();
        }
    }
}

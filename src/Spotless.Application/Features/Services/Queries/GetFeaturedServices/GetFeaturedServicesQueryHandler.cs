using MediatR;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Services.Queries.GetFeaturedServices
{
    public class GetFeaturedServicesQueryHandler : IRequestHandler<GetFeaturedServicesQuery, IReadOnlyList<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceMapper _serviceMapper;

        public GetFeaturedServicesQueryHandler(IUnitOfWork unitOfWork, IServiceMapper serviceMapper)
        {
            _unitOfWork = unitOfWork;
            _serviceMapper = serviceMapper;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(GetFeaturedServicesQuery request, CancellationToken cancellationToken)
        {
            var featuredServices = await _unitOfWork.Services.GetPagedAsync(
                filter: s => true,
                skip: 0,
                take: request.Count,
                include: null,
                orderBy: q => q.OrderBy(s => s.Name)
            );

            if (!featuredServices.Any())
            {
                return new List<ServiceDto>();
            }


            return _serviceMapper.MapToDto(featuredServices).ToList();
        }
    }
}
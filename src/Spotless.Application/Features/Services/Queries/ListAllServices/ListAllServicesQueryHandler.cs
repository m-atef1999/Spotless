using MediatR;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Application.Services;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Services.Queries.ListAllServices
{

    public class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, PagedResponse<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceMapper _serviceMapper;
        private readonly CachedServiceService _cachedServiceService;

        public ListServicesQueryHandler(IUnitOfWork unitOfWork, IServiceMapper serviceMapper, CachedServiceService cachedServiceService)
        {
            _unitOfWork = unitOfWork;
            _serviceMapper = serviceMapper;
            _cachedServiceService = cachedServiceService;
        }

        public async Task<PagedResponse<ServiceDto>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.NameSearchTerm))
            {
                var cachedServices = await _cachedServiceService.GetAllServicesAsync();
                var pagedCached = cachedServices.Skip(request.Skip).Take(request.PageSize);
                return new PagedResponse<ServiceDto>(pagedCached.ToList(), cachedServices.Count(), request.PageNumber, request.PageSize);
            }

            var filterExpression = BuildFilterExpression(request);
            var totalRecords = await _unitOfWork.Services.CountAsync(filterExpression);
            var services = await _unitOfWork.Services.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize,
                include: null,
                orderBy: q => q.OrderBy(s => s.Name)
            );

            var serviceDtos = _serviceMapper.MapToDto(services).ToList();
            return new PagedResponse<ServiceDto>(serviceDtos, totalRecords, request.PageNumber, request.PageSize);
        }


        private Expression<Func<Service, bool>> BuildFilterExpression(ListServicesQuery request)
        {
            return service =>
                string.IsNullOrEmpty(request.NameSearchTerm) || service.Name.Contains(request.NameSearchTerm!);
        }
    }
}
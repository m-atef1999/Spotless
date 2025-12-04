using MediatR;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Application.Services;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Services.Queries.ListAllServices
{

    public class ListServicesQueryHandler(IUnitOfWork unitOfWork, IServiceMapper serviceMapper, CachedServiceService cachedServiceService) : IRequestHandler<ListServicesQuery, PagedResponse<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IServiceMapper _serviceMapper = serviceMapper;
        private readonly CachedServiceService _cachedServiceService = cachedServiceService;

        public async Task<PagedResponse<ServiceDto>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
        {
            var searchTerm = request.NameSearchTerm?.Trim();
            
            // If no search term, use cached services
            if (string.IsNullOrEmpty(searchTerm))
            {
                var cachedServices = await _cachedServiceService.GetAllServicesAsync();
                var pagedCached = cachedServices.Skip(request.Skip).Take(request.PageSize);
                return new PagedResponse<ServiceDto>([.. pagedCached], cachedServices.Count(), request.PageNumber, request.PageSize);
            }

            // Use cached services and filter in-memory for search (supports Unicode/Arabic)
            var allServices = await _cachedServiceService.GetAllServicesAsync();
            var filteredServices = allServices
                .Where(s => s.Name != null && s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            var pagedFiltered = filteredServices.Skip(request.Skip).Take(request.PageSize).ToList();
            return new PagedResponse<ServiceDto>(pagedFiltered, filteredServices.Count, request.PageNumber, request.PageSize);
        }
    }
}

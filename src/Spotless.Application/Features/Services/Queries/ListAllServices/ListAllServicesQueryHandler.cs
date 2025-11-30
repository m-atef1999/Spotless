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
            if (string.IsNullOrEmpty(request.NameSearchTerm))
            {
                var cachedServices = await _cachedServiceService.GetAllServicesAsync();
                var pagedCached = cachedServices.Skip(request.Skip).Take(request.PageSize);
                return new PagedResponse<ServiceDto>([.. pagedCached], cachedServices.Count(), request.PageNumber, request.PageSize);
            }

            var filterExpression = BuildFilterExpression(request);
            var totalRecords = await _unitOfWork.Services.CountAsync(filterExpression);
            var services = await _unitOfWork.Services.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize,
                include: q => q.Include(s => s.Category),
                orderBy: q => q.OrderBy(s => s.Name)
            );

            var serviceDtos = _serviceMapper.MapToDto(services).ToList();
            return new PagedResponse<ServiceDto>(serviceDtos, totalRecords, request.PageNumber, request.PageSize);
        }


        private Expression<Func<Service, bool>> BuildFilterExpression(ListServicesQuery request)
        {
            var raw = request.NameSearchTerm?.Trim();
                var searchTerm = request.NameSearchTerm?.Trim().ToLowerInvariant();
                return service =>
                    string.IsNullOrEmpty(searchTerm) ||
                    (service.Name != null && service.Name.ToLowerInvariant().Contains(searchTerm));
        }
    }
}

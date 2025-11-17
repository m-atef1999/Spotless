using MediatR;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Services.Queries
{

    public class ListServicesByCategoryQueryHandler : IRequestHandler<ListServicesByCategoryQuery, PagedResponse<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceMapper _serviceMapper;


        public ListServicesByCategoryQueryHandler(IUnitOfWork unitOfWork, IServiceMapper serviceMapper)
        {
            _unitOfWork = unitOfWork;
            _serviceMapper = serviceMapper;
        }

        public async Task<PagedResponse<ServiceDto>> Handle(ListServicesByCategoryQuery request, CancellationToken cancellationToken)
        {

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


            return new PagedResponse<ServiceDto>(
                serviceDtos,
                totalRecords,
                request.PageNumber,
                request.PageSize
            );
        }


        private Expression<Func<Service, bool>> BuildFilterExpression(ListServicesByCategoryQuery request)
        {

            return service =>
                service.CategoryId == request.CategoryId &&

                (string.IsNullOrEmpty(request.NameSearchTerm) || service.Name.Contains(request.NameSearchTerm!));
        }
    }
}
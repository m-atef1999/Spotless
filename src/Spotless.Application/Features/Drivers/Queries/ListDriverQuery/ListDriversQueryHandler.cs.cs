using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Drivers.Queries.ListDriverQuery
{

    public class ListDriversQueryHandler(IUnitOfWork unitOfWork, IDriverMapper driverMapper) : IRequestHandler<ListDriversQuery, PagedResponse<DriverDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDriverMapper _driverMapper = driverMapper;

        public async Task<PagedResponse<DriverDto>> Handle(ListDriversQuery request, CancellationToken cancellationToken)
        {

            var filterExpression = BuildFilterExpression(request);


            var totalRecords = await _unitOfWork.Drivers.CountAsync(filterExpression);


            var drivers = await _unitOfWork.Drivers.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize,


                orderBy: q => q.OrderBy(d => d.Name)
            );


            var driverDtos = _driverMapper.MapToProfileDto(drivers).ToList();


            return new PagedResponse<DriverDto>(
                driverDtos,
                totalRecords,
                request.PageNumber,
                request.PageSize
            );
        }

        private Expression<Func<Driver, bool>> BuildFilterExpression(ListDriversQuery request)
        {

            return driver =>


                (!request.StatusFilter.HasValue || driver.Status == request.StatusFilter.Value) &&


                (string.IsNullOrEmpty(request.NameSearchTerm) ||
                 (driver.Name != null && driver.Name.ToLowerInvariant().Contains(request.NameSearchTerm!.Trim().ToLowerInvariant())));
        }
    }
}
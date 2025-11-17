using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Drivers.Queries
{
    public class ListDriversQueryHandler : IRequestHandler<ListDriversQuery, PagedResponse<DriverProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverMapper _driverMapper;

        public ListDriversQueryHandler(IUnitOfWork unitOfWork, IDriverMapper driverMapper)
        {
            _unitOfWork = unitOfWork;
            _driverMapper = driverMapper;
        }

        public async Task<PagedResponse<DriverProfileDto>> Handle(ListDriversQuery request, CancellationToken cancellationToken)
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


            return new PagedResponse<DriverProfileDto>(
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

                (string.IsNullOrEmpty(request.NameSearchTerm) || driver.Name.Contains(request.NameSearchTerm!));
        }
    }
}
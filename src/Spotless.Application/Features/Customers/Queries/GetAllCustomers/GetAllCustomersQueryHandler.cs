using MediatR;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Customers.Queries.GetAllCustomers
{
    public class ListCustomersQueryHandler : IRequestHandler<ListCustomersQuery, PagedResponse<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerMapper _customerMapper;

        public ListCustomersQueryHandler(IUnitOfWork unitOfWork, ICustomerMapper customerMapper)
        {
            _unitOfWork = unitOfWork;
            _customerMapper = customerMapper;
        }

        public async Task<PagedResponse<CustomerDto>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
        {

            var filterExpression = BuildFilterExpression(request);

            var totalRecords = await _unitOfWork.Customers.CountAsync(filterExpression);

            var customers = await _unitOfWork.Customers.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize,
                include: null,
                orderBy: q => q.OrderBy(c => c.Name)
            );


            var customerDtos = _customerMapper.MapToDto(customers).ToList();


            return new PagedResponse<CustomerDto>(
                customerDtos,
                totalRecords,
                request.PageNumber,
                request.PageSize
            );
        }


        private Expression<Func<Customer, bool>> BuildFilterExpression(ListCustomersQuery request)
        {
            return customer =>
                (string.IsNullOrEmpty(request.NameFilter) || customer.Name.Contains(request.NameFilter!)) &&
                (string.IsNullOrEmpty(request.EmailFilter) || customer.Email.Contains(request.EmailFilter!));
        }
    }
}
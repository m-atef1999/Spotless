using MediatR;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Customers.Queries.GetCustomerProfile
{
    public class GetCustomerProfileQueryHandler(IUnitOfWork unitOfWork, ICustomerMapper customerMapper) : IRequestHandler<GetCustomerProfileQuery, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICustomerMapper _customerMapper = customerMapper;

        public async Task<CustomerDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
        {

            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId) ?? throw new KeyNotFoundException($"Customer profile not found for ID: {request.CustomerId}");
            return _customerMapper.MapToDto(customer);
        }
    }
}
using MediatR;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Customers
{
    public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerMapper _customerMapper;

        public GetCustomerProfileQueryHandler(IUnitOfWork unitOfWork, ICustomerMapper customerMapper)
        {
            _unitOfWork = unitOfWork;
            _customerMapper = customerMapper;
        }

        public async Task<CustomerDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
        {

            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);

            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer profile not found for ID: {request.CustomerId}");
            }

            return _customerMapper.MapToDto(customer);
        }
    }
}
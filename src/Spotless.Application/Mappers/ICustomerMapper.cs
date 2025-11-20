using Spotless.Application.Dtos.Customer;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public interface ICustomerMapper
    {
        CustomerDto MapToDto(Customer entity);
        IEnumerable<CustomerDto> MapToDto(IEnumerable<Customer> entities);


    }
}

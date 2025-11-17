using Spotless.Application.Dtos.Customer;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Mappers
{

    public class CustomerMapper : ICustomerMapper
    {

        public CustomerDto MapToDto(Customer customer)
        {
            if (customer == null) return null!;

            var address = customer.Address;

            return new CustomerDto(
                Id: customer.Id,
                Name: customer.Name,
                Phone: customer.Phone,
                Email: customer.Email,


                Street: address.Street,
                City: address.City,
                Country: address.Country,
                ZipCode: address.ZipCode,


                WalletBalance: customer.WalletBalance.Amount,
                WalletCurrency: customer.WalletBalance.Currency,
                Type: customer.Type
            );
        }

        public IEnumerable<CustomerDto> MapToDto(IEnumerable<Customer> entities)
        {
            return entities.Select(MapToDto);
        }


    }
}
using Spotless.Application.Dtos.Customer;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return new CustomerDto(
                Id: customer.Id,
                Name: customer.Name,
                Phone: customer.Phone,
                Email: customer.Email,
                Address: customer.Address,
                WalletBalance: customer.WalletBalance.Amount,
                Type: customer.Type);
        }


        public static void UpdateFromDto(this Customer customer, UpdateCustomerDto dto)
        {
            customer.UpdateProfile(dto.Name, dto.Phone, dto.Address);
        }
    }
}

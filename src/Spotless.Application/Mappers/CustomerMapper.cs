using Spotless.Application.Dtos.Customer;
using Spotless.Domain.Entities;

namespace Spotless.Application.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer customer)
        {

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

    }
}
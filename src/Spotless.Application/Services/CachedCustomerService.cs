using Spotless.Application.Dtos.Customer;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Services
{
    public class CachedCustomerService(ICustomerRepository customerRepository, ICachingService cachingService)
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ICachingService _cachingService = cachingService;
        private const string CUSTOMERS_CACHE_KEY = "customers:all";

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<CustomerDto>>(CUSTOMERS_CACHE_KEY);
            if (cached != null) return cached;

            var customers = await _customerRepository.GetAllAsync();
            var customerDtos = customers.Select(c => new CustomerDto(
                c.Id,
                c.Name,
                c.Phone,
                c.Email,
                c.Address.Street,
                c.Address.City,
                c.Address.Country,
                c.Address.ZipCode,
                c.WalletBalance.Amount,
                c.WalletBalance.Currency,
                c.Type
            ));

            await _cachingService.SetAsync(CUSTOMERS_CACHE_KEY, customerDtos, TimeSpan.FromHours(2));
            return customerDtos;
        }

        public async Task InvalidateCustomerCacheAsync()
        {
            await _cachingService.RemoveAsync(CUSTOMERS_CACHE_KEY);
        }
    }
}

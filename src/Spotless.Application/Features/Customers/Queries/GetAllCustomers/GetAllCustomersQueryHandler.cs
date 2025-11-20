using MediatR;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Customers.Queries.GetAllCustomers
{
    public class ListCustomersQueryHandler(CachedCustomerService cachedCustomerService) 
        : IRequestHandler<ListCustomersQuery, PagedResponse<CustomerDto>>
    {
        private readonly CachedCustomerService _cachedCustomerService = cachedCustomerService;

        public async Task<PagedResponse<CustomerDto>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
        {
            var cachedCustomers = await _cachedCustomerService.GetAllCustomersAsync();

            // Apply filters
            var filtered = cachedCustomers.AsEnumerable();

            if (!string.IsNullOrEmpty(request.NameFilter))
            {
                filtered = filtered.Where(c => c.Name.Contains(request.NameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(request.EmailFilter))
            {
                filtered = filtered.Where(c => c.Email.Contains(request.EmailFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Apply pagination
            var pagedCustomers = filtered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<CustomerDto>(
                pagedCustomers,
                filtered.Count(),
                request.PageNumber,
                request.PageSize
            );
        }
    }
}

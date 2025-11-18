using Spotless.Application.Dtos.Customer;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Customers.Queries.GetAllCustomers
{

    public record ListCustomersQuery(
        string? NameFilter,
        string? EmailFilter
    ) : PaginationBaseRequest, IQuery<PagedResponse<CustomerDto>>;
}
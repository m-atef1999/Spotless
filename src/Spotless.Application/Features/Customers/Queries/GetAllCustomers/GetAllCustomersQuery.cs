using MediatR;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
namespace Spotless.Application.Features.Customers
{

    public record ListCustomersQuery(
        string? NameFilter,
        string? EmailFilter
    ) : PaginationBaseRequest, IRequest<PagedResponse<CustomerDto>>;
}
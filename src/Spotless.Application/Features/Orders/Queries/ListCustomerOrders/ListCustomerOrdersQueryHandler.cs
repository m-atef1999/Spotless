using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Orders.Queries.ListCustomerOrders
{
    public class ListCustomerOrdersQueryHandler : IRequestHandler<ListCustomerOrdersQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListCustomerOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(ListCustomerOrdersQuery request, CancellationToken cancellationToken)
        {

            var orders = await _unitOfWork.Orders.GetAsync(o => o.CustomerId == request.CustomerId);

            if (orders == null || !orders.Any())
            {
                return new List<OrderDto>();
            }

            return orders.Select(o => o.ToDto()).ToList();
        }
    }
}

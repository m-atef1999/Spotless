using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Domain.Exceptions;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork, IOrderMapper mapper) : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _mapper = mapper;

        public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new NotFoundException($"Order with ID {request.OrderId} not found.");
            }

            order.UpdateStatus(request.NewStatus);
            await _unitOfWork.CommitAsync();

            return _mapper.MapToDto(order);
        }
    }
}

using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.ConfirmOrder.Commands
{
    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            }


            if (order.Status != OrderStatus.Requested)
            {
                throw new InvalidOperationException($"Order ID {request.OrderId} cannot be confirmed because its current status is {order.Status}. Only '{OrderStatus.Requested}' orders can be confirmed.");
            }


            order.SetStatus(OrderStatus.Confirmed);


            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Payments.Commands.InitiatePayment
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService;

        public InitiatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentGatewayService paymentGatewayService)
        {
            _unitOfWork = unitOfWork;
            _paymentGatewayService = paymentGatewayService;
        }

        public async Task<string> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);

            if (order == null || customer == null)
                throw new KeyNotFoundException("Order or Customer not found.");

            if (order.Status != OrderStatus.Requested)
            {
                throw new InvalidOperationException($"Payment can only be initiated for orders in the '{OrderStatus.Requested}' status. Current status is {order.Status}.");
            }


            var payment = new Payment(
                request.CustomerId,
                request.OrderId,
                order.TotalPrice,
                order.PaymentMethod
            );

            await _unitOfWork.Payments.AddAsync(payment);


            string paymentUrl = await _paymentGatewayService.InitiatePaymentAsync(
                order.Id,
                order.TotalPrice,
                customer.Email,
                cancellationToken
            );

            await _unitOfWork.CommitAsync();

            return paymentUrl;
        }
    }
}

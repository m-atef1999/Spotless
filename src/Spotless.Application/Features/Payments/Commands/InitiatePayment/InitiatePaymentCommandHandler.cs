using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Payments.Commands.InitiatePayment
{
    public class InitiatePaymentCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGatewayService) : IRequestHandler<InitiatePaymentCommand, InitiatePaymentResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;

        public async Task<InitiatePaymentResult> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            // Get order and validate
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId)
                ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

            // Get customer
            var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId)
                ?? throw new KeyNotFoundException("Customer not found.");

            // Validate order status
            if (order.Status != OrderStatus.Requested)
            {
                throw new InvalidOperationException(
                    $"Payment can only be initiated for orders in 'Requested' status. Current status is {order.Status}.");
            }

            // Create payment record
            var payment = new Payment(
                customerId: order.CustomerId,
                amount: order.TotalPrice,
                method: request.PaymentMethod,
                orderId: order.Id
            );

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.CommitAsync();

            // Initiate payment with gateway
            string paymentUrl = await _paymentGatewayService.InitiatePaymentAsync(
                payment.Id,
                order.TotalPrice,
                customer.Email,
                cancellationToken
            );

            // Extract transaction reference from URL (mock implementation returns it in URL)
            var transactionReference = payment.Id.ToString();

            await _unitOfWork.CommitAsync();

            return new InitiatePaymentResult(
                PaymentId: payment.Id,
                PaymentUrl: paymentUrl,
                TransactionReference: transactionReference
            );
        }
    }
}

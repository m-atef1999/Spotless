using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;
using Spotless.Domain.Exceptions;

namespace Spotless.Application.Features.Payments.Commands.ProcessWebhook
{
    public class ProcessWebhookCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGatewayService,
        IDomainEventPublisher eventPublisher,
        IPaymobSignatureService paymobSignatureService) : IRequestHandler<ProcessWebhookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;
        private readonly IDomainEventPublisher _eventPublisher = eventPublisher;
        private readonly IPaymobSignatureService _paymobSignatureService = paymobSignatureService;

        public async Task<Unit> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            // Verify the HMAC signature to ensure the webhook is authentic
            var isSignatureValid = _paymobSignatureService.VerifyProcessedCallbackSignature(
                request.CallbackData, 
                request.HmacSignature);

            if (!isSignatureValid)
            {
                throw new UnauthorizedException("Invalid webhook signature. The webhook request is not authentic.");
            }

            // Extract the Paymob transaction ID and order information
            var transactionId = request.CallbackData.Id.ToString();
            var orderId = request.CallbackData.OrderId.ToString();

            if (request.CallbackData.Id == 0)
            {
                throw new ArgumentException("Paymob transaction ID is missing from webhook payload.");
            }


            if (!Guid.TryParse(orderId, out Guid paymentOrderId))
            {
                throw new ArgumentException($"Invalid order ID format: {orderId}");
            }

            // Find payment by order ID using the repository method
            var payments = await _unitOfWork.Payments.GetPaymentsByOrderIdAsync(paymentOrderId);
            var payment = payments.FirstOrDefault() ?? throw new KeyNotFoundException($"No payment record found for order ID {paymentOrderId}");

            // If payment is already processed, return early
            if (payment.Status != PaymentStatus.Pending)
            {
                return Unit.Value;
            }

            // Update payment status based on Paymob webhook data
            if (request.CallbackData.Success && !request.CallbackData.Pending)
            {
                // Payment completed successfully
                payment.CompletePayment();
                payment.SetExternalTransaction(transactionId, "Paymob");

                if (payment.OrderId.HasValue)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value);
                    if (order != null)
                    {
                        order.SetStatus(OrderStatus.Confirmed);
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }

                // Publish domain event
                var paymentCompletedEvent = payment.CreatePaymentCompletedEvent();
                await _eventPublisher.PublishAsync(paymentCompletedEvent);
            }
            else if (request.CallbackData.ErrorOccured || !request.CallbackData.Success)
            {
                // Payment failed or had errors
                payment.FailPayment();
                payment.SetExternalTransaction(transactionId, "Paymob");

                if (payment.OrderId.HasValue)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value);
                    if (order != null)
                    {
                        order.SetStatus(OrderStatus.PaymentFailed);
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }
            }
            // If payment is still pending, do nothing

            // Save changes
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Payments
{
    public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService;

        public ProcessWebhookCommandHandler(IUnitOfWork unitOfWork, IPaymentGatewayService paymentGatewayService)
        {
            _unitOfWork = unitOfWork;
            _paymentGatewayService = paymentGatewayService;
        }

        public async Task<Unit> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            PaymentStatus finalStatus = await _paymentGatewayService
                .VerifyPaymentAsync(request.PaymentReference, cancellationToken);

            if (!Guid.TryParse(request.PaymentReference, out Guid paymentId))
                throw new ArgumentException("Invalid payment reference format.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);

            if (payment == null)
                throw new KeyNotFoundException($"Payment record with ID {paymentId} not found.");

            if (finalStatus == PaymentStatus.Completed)
            {
                payment.CompletePayment();


                if (payment.OrderId.HasValue)
                {

                    var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value);

                    if (order != null)
                    {
                        order.SetStatus(OrderStatus.PickedUp);
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }
            }
            else if (finalStatus == PaymentStatus.Failed)
            {
                // TODO: Add logic here to handle failed payment status (e.g., payment.FailPayment())
            }

            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

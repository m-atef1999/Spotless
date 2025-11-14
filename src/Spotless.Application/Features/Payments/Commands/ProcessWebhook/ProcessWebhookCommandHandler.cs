using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Payments.Commands.ProcessWebhook
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
            // 1. Verify status with the external gateway
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


                var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);
                if (order != null)
                {

                    order.SetStatus(OrderStatus.PickedUp);
                    await _unitOfWork.Orders.UpdateAsync(order);
                }
            }
            else if (finalStatus == PaymentStatus.Failed)
            {

            }

            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

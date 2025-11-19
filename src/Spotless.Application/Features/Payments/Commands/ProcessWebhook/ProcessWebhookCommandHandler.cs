using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Payments.Commands.ProcessWebhook
{
    public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IDomainEventPublisher _eventPublisher;

        public ProcessWebhookCommandHandler(IUnitOfWork unitOfWork, IPaymentGatewayService paymentGatewayService, IDomainEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _paymentGatewayService = paymentGatewayService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {

            PaymentStatus finalStatus = await _paymentGatewayService
                .VerifyPaymentAsync(request.PaymentReference, cancellationToken);


            if (!Guid.TryParse(request.PaymentReference, out Guid paymentId))
                throw new ArgumentException("Invalid payment reference format. Expected a GUID.", nameof(request.PaymentReference));

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);

            if (payment == null)
                throw new KeyNotFoundException($"Payment record with ID {paymentId} not found.");


            if (payment.Status != PaymentStatus.Pending)
            {
                return Unit.Value;
            }


            if (finalStatus == PaymentStatus.Completed)
            {
                payment.CompletePayment();

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
            else if (finalStatus == PaymentStatus.Failed)
            {

                payment.FailPayment();


                if (payment.OrderId.HasValue)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value);
                    order?.SetStatus(OrderStatus.PaymentFailed);
                    if (order != null) await _unitOfWork.Orders.UpdateAsync(order);
                }
            }


            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
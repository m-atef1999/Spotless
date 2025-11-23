using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;
using Spotless.Application.Features.Payments.Commands.InitiatePayment;
using Microsoft.Extensions.Logging;
using Spotless.Domain.Entities;

namespace Spotless.Application.Features.Customers.Commands.TopUpWallet
{
    public class TopUpWalletCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentGatewayService paymentGatewayService,
        INotificationService notificationService,
        IAuthService authService,
        Microsoft.Extensions.Logging.ILogger<TopUpWalletCommandHandler> logger) : IRequestHandler<TopUpWalletCommand, InitiatePaymentResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;
        private readonly Microsoft.Extensions.Logging.ILogger<TopUpWalletCommandHandler> _logger = logger;

        public async Task<InitiatePaymentResult> Handle(TopUpWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId) ?? throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
                var topUpAmount = new Money(request.Request.AmountValue, "EGP"); // Assuming EGP currency

                // 1. Create Payment Entity Manually (Decoupled from Customer collection to avoid tracking issues)
                var payment = new Payment(
                    customerId: customer.Id,
                    amount: topUpAmount,
                    method: request.Request.PaymentMethod,
                    orderId: null,
                    adminId: null
                );

                // 2. Initiate Payment with Gateway
                string paymentUrl = await _paymentGatewayService.InitiatePaymentAsync(
                    null, // No Order ID for Wallet TopUp
                    topUpAmount,
                    customer.Email,
                    cancellationToken
                );

                var transactionReference = payment.Id.ToString();
                payment.SetExternalTransaction(transactionReference, "Paymob");

                // 3. Log Amount for Sanity Check
                _logger.LogInformation("Creating payment for customer {CustomerId} with amount {Amount}", request.CustomerId, payment.Amount.Amount);

                // 4. Add Payment to Repository explicitly
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.CommitAsync();

                // Send Notification
                try
                {
                    var userId = await _authService.GetUserIdByCustomerIdAsync(request.CustomerId);
                    if (userId != null)
                    {
                        await _notificationService.SendPushNotificationAsync(userId, "Top-up Initiated", $"A wallet top-up of {request.Request.AmountValue} EGP has been initiated.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send top-up notification for Customer {CustomerId}", request.CustomerId);
                }

                return new InitiatePaymentResult(
                    PaymentId: payment.Id,
                    PaymentUrl: paymentUrl,
                    TransactionReference: transactionReference
                );
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                _logger.LogError(ex, "Error processing wallet top-up for customer {CustomerId}. Inner: {InnerMessage}", request.CustomerId, innerMessage);
                throw;
            }
        }
    }
}

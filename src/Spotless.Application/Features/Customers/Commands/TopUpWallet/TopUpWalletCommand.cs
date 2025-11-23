using MediatR;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Features.Payments.Commands.InitiatePayment;

namespace Spotless.Application.Features.Customers.Commands.TopUpWallet
{

    public record TopUpWalletCommand(
        Guid CustomerId,
        WalletTopUpRequest Request
    ) : IRequest<InitiatePaymentResult>;
}

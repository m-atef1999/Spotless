using MediatR;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Features.Customers.Commands.TopUpWallet
{

    public record TopUpWalletCommand(
        Guid CustomerId,
        WalletTopUpRequest Request
    ) : IRequest<Unit>;
}

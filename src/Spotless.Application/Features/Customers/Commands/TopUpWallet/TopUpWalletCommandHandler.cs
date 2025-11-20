using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Customers.Commands.TopUpWallet
{
    public class TopUpWalletCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<TopUpWalletCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(TopUpWalletCommand request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId) ?? throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
            var topUpAmount = new Money(request.Request.AmountValue, "EGP"); // Assuming EGP currency


            customer.DepositFunds(topUpAmount, request.Request.PaymentMethod);

            await _unitOfWork.Customers.UpdateAsync(customer);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
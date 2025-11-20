using FluentValidation;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Validation
{
    public class WalletTopUpRequestValidator : AbstractValidator<WalletTopUpRequest>
    {
        public WalletTopUpRequestValidator()
        {
            RuleFor(x => x.AmountValue)
                .GreaterThanOrEqualTo(10.0M).WithMessage("Top-up amount must be at least 10 EGP.")
                .LessThanOrEqualTo(100000.0M).WithMessage("Top-up amount cannot exceed 100,000 EGP.")
                .Must(value => DecimalPlaces(value) <= 2)
                    .WithMessage("Amount value can only have up to 2 decimal places.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method specified.");
        }

        private int DecimalPlaces(decimal amount)
        {
            amount = Math.Abs(amount);
            amount -= Math.Truncate(amount);
            return BitConverter.GetBytes(decimal.GetBits(amount)[3])[2];
        }
    }
}

using Spotless.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Dtos.Customers
{

    public record TopUpWalletRequest
    {

        [Required]
        [Range(10.0, 100000.0, ErrorMessage = "Top-up amount must be between 10 EGP and 100,000 EGP.")]
        public decimal AmountValue { get; init; }

        [Required]

        public PaymentMethod PaymentMethod { get; init; }
    }
}
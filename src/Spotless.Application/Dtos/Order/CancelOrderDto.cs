using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Dtos.Orders
{

    public record CancelOrderRequest
    {
        [Required]
        public Guid OrderId { get; init; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "A cancellation reason must be between 10 and 500 characters.")]
        public string CancellationReason { get; init; } = string.Empty;
    }
}
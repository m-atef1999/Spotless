using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Dtos.Responses
{

    public record PriceItemResponse
    {
        [Required]
        public Guid ServiceId { get; init; }

        [Required]
        public decimal ItemPrice { get; init; }

        [Required]
        public string Currency { get; init; } = string.Empty;
    }
}

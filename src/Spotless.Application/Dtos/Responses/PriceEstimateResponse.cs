using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Dtos.Responses
{

    public record PriceEstimateResponse
    {

        [Required]
        public decimal TotalPrice { get; init; }


        [Required]
        public string Currency { get; init; } = string.Empty;

        public IReadOnlyList<PriceItemResponse> ItemBreakdown { get; init; } = new List<PriceItemResponse>();
    }
}
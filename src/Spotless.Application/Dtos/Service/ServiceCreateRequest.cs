namespace Spotless.Application.Dtos.Service
{
    public record CreateServiceDto(
        Guid CategoryId,
        string Name,
        string Description,
        decimal PricePerUnitAmount,
        string PricePerUnitCurrency,
        decimal EstimatedDurationHours,
        decimal MaxWeightKg = 50m,
        string? ImageUrl = null
    );
}



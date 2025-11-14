namespace Spotless.Application.Dtos.Service
{
    public record ServiceDto(
        Guid Id,
        Guid CategoryId,
        string Name,
        string Description,
        decimal PricePerUnitAmount,
        string PricePerUnitCurrency,
        decimal EstimatedDurationHours
    );
}
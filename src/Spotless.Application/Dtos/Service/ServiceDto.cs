using Spotless.Domain.ValueObjects;

public record ServiceDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string Description,
    Money PricePerUnit,
    decimal EstimatedDurationHours
);
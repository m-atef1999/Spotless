using Spotless.Domain.ValueObjects;
public record PricingDetailsDto(

    Guid ServiceCategoryId,


    Location ServiceLocation,


    IReadOnlyList<PricingItemDto> ServiceItems
);

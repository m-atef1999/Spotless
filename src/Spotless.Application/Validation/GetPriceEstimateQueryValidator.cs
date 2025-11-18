using FluentValidation;
using Spotless.Application.Features.Orders.Queries.GetPriceEstimate;


namespace Spotless.Application.Validation
{


    public class GetPriceEstimateQueryValidator : AbstractValidator<GetPriceEstimateQuery>
    {
        public GetPriceEstimateQueryValidator()
        {
            RuleFor(x => x.Details)
                .NotNull().WithMessage("Pricing details are required for an estimate.")
                .SetValidator(new PricingDetailsDtoValidator());
        }
    }


    public class PricingDetailsDtoValidator : AbstractValidator<PricingDetailsDto> // Correctly targets the defined DTO
    {
        public PricingDetailsDtoValidator()
        {

            RuleFor(x => x.ServiceCategoryId)
            .NotEmpty().WithMessage("Service Category ID is required.");


            RuleFor(x => x.ServiceLocation)
                .NotNull().WithMessage("Service location is required.");


            RuleFor(x => x.ServiceLocation.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.ServiceLocation.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");


            RuleFor(x => x.ServiceItems)
                .NotEmpty().WithMessage("At least one item is required for the estimate.")
                .Must(items => items.Any()).WithMessage("The service items list cannot be empty.");


            RuleForEach(x => x.ServiceItems)
                .SetValidator(new PricingItemDtoValidator());
        }
    }


    public class PricingItemDtoValidator : AbstractValidator<PricingItemDto>
    {
        public PricingItemDtoValidator()
        {

            RuleFor(x => x.ItemName)
                .NotEmpty().WithMessage("Item name is required.");


            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
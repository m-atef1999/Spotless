using FluentValidation;
using Spotless.Application.Features.Orders.Queries.GetPriceEstimate;

namespace Spotless.Application.Validation
{
    public class GetPriceEstimateQueryValidator : AbstractValidator<GetPriceEstimateQuery>
    {
        public GetPriceEstimateQueryValidator()
        {
            RuleFor(x => x.OrderDto)
                .NotNull().WithMessage("Order details are required for an estimate.")
                .SetValidator(new CreateOrderDtoValidator());
        }
    }
}

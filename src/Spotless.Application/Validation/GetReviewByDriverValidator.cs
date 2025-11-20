using FluentValidation;
using Spotless.Application.Features.Reviews.Queries.GetReviewByDriver;

namespace Spotless.Application.Validation
{

    public class GetReviewsByDriverQueryValidator : AbstractValidator<GetReviewsByDriverQuery>
    {
        public GetReviewsByDriverQueryValidator()
        {
            RuleFor(x => x.DriverId)
                .NotEmpty().WithMessage("Driver ID is required to fetch associated reviews.");
        }
    }
}

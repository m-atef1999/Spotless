using FluentValidation;
using Spotless.Application.Features.Reviews.Commands.CreateReview;

namespace Spotless.Application.Validation
{

    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID must be provided.");


            RuleFor(x => x.Dto).NotNull().WithMessage("Review details are required.");

            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.OrderId)
                    .NotEmpty().WithMessage("Order ID is required for the review.");

                RuleFor(x => x.Dto.Rating)
                    .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

                RuleFor(x => x.Dto.Comment)
                    .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
            });
        }
    }
}
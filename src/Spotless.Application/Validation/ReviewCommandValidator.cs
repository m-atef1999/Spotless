using FluentValidation;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Features.Reviews;

namespace Spotless.Application.Validation
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator(IOptions<ReviewSettings> settings)
        {
            var reviewSettings = settings.Value;

            RuleFor(x => x.Dto.Comment)
                .MaximumLength(reviewSettings.MaxCommentLength)
                .WithMessage($"Comment must not exceed {reviewSettings.MaxCommentLength} characters.");

        }
    }
}

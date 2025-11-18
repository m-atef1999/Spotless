using FluentValidation;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Validation
{

    public class SubmitDriverApplicationValidator : AbstractValidator<SubmitDriverApplicationDto>
    {
        public SubmitDriverApplicationValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Must be a valid email address.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10,}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.VehicleInfo)
                .NotEmpty().WithMessage("Vehicle information is required.")
                .MaximumLength(200).WithMessage("Vehicle information cannot exceed 200 characters.");
        }
    }
}
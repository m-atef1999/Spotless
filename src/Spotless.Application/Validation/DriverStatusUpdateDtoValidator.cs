using FluentValidation;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Validation
{
    public class DriverStatusUpdateDtoValidator : AbstractValidator<DriverStatusUpdateDto>
    {
        public DriverStatusUpdateDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "Available" || s == "Busy" || s == "Offline")
                .WithMessage("Status must be Available, Busy, or Offline.");
        }
    }
}

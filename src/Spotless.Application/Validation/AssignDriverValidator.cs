using FluentValidation;
using Spotless.Application.Features.Drivers.Commands.AssignDriver;

namespace Spotless.Application.Validation
{

    public class AssignDriverCommandValidator : AbstractValidator<AssignDriverCommand>
    {
        public AssignDriverCommandValidator()
        {

            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("The Order ID is required for driver assignment.");


            RuleFor(x => x.DriverId)
                .NotEmpty().WithMessage("The Driver ID is required for assignment.");
        }
    }
}

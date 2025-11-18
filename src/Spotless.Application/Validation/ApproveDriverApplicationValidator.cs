using FluentValidation;
using Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication;

namespace Spotless.Application.Validation
{

    public class ApproveDriverApplicationCommandValidator : AbstractValidator<ApproveDriverApplicationCommand>
    {
        public ApproveDriverApplicationCommandValidator()
        {

            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("The Application ID (Driver ID) is required for approval.");


            RuleFor(x => x.AdminId)
                .NotEmpty().WithMessage("The Admin ID is required to track which administrator approved the application.");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A password is required to create the driver's identity.")
                .MinimumLength(8).WithMessage("The initial password must be at least 8 characters long.")
                .MaximumLength(50).WithMessage("Password cannot exceed 50 characters.");

        }
    }
}
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




        }
    }
}

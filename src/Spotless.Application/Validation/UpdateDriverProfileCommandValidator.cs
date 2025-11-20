using FluentValidation;
using Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile;

namespace Spotless.Application.Validation
{
    public class UpdateDriverProfileCommandValidator : AbstractValidator<UpdateDriverProfileCommand>
    {
        public UpdateDriverProfileCommandValidator()
        {

            RuleFor(command => command.Dto).NotNull().WithMessage("Driver profile data is required.");


            RuleFor(command => command.Dto.Name)
                .NotEmpty().WithMessage("Driver name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(command => command.Dto.VehicleInfo)
                .NotEmpty().WithMessage("Vehicle information is required.")
                .MaximumLength(250).WithMessage("Vehicle information cannot exceed 250 characters.");


            RuleFor(command => command.Dto.Phone)
                            .MaximumLength(15).When(command => !string.IsNullOrEmpty(command.Dto.Phone))
                            .WithMessage("Phone number cannot exceed 15 digits.");
        }
    }
}

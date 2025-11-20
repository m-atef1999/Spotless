using FluentValidation;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Validation
{
    public class CreateServiceRequestValidator : AbstractValidator<CreateServiceDto>
    {
        public CreateServiceRequestValidator()
        {


            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");



            RuleFor(x => x.PricePerUnitAmount)
                .GreaterThan(0).WithMessage("Price per unit must be greater than zero.");

            RuleFor(x => x.PricePerUnitCurrency)
                .NotEmpty().WithMessage("Currency code is required.")
                .Length(3).WithMessage("Currency code must be 3 characters (e.g., USD).");

            RuleFor(x => x.EstimatedDurationHours)

                .GreaterThan(0).WithMessage("Estimated duration must be positive.");

            RuleFor(x => x.MaxWeightKg)
                .GreaterThan(0).WithMessage("Max weight must be greater than zero.")
                .LessThanOrEqualTo(1000).WithMessage("Max weight seems unreasonably large.");
        }
    }
}

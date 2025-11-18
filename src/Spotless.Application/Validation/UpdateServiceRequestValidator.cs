using FluentValidation;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Validation
{
    public class UpdateServiceRequestValidator : AbstractValidator<UpdateServiceDto>
    {
        public UpdateServiceRequestValidator()
        {

            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage("Service ID is required for updating.");


            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));


            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));


            RuleFor(x => x.PricePerUnitValue)
                .GreaterThan(0.0M).WithMessage("Price must be a positive value.")
                .InclusiveBetween(0.01M, 99999.99M).WithMessage("Price must be between $0.01 and $99,999.99.")
                .When(x => x.PricePerUnitValue.HasValue);


            RuleFor(x => x.EstimatedDurationHours)
                .InclusiveBetween(0.1M, 10.0M).WithMessage("Duration must be between 0.1 and 10 hours.")
                .When(x => x.EstimatedDurationHours.HasValue);


            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID cannot be empty if provided.")
                .When(x => x.CategoryId.HasValue);


            RuleFor(x => x.PricePerUnitCurrency)
                .NotEmpty().WithMessage("Currency must be provided if Price is being updated.")
                .When(x => x.PricePerUnitValue.HasValue);
        }
    }
}
using FluentValidation;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Validation
{
    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Phone)
                .Length(10, 15).When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Phone number must be between 10 and 15 digits.");

            RuleFor(x => x.City).NotEmpty().WithMessage("City is required for update.");
        }
    }
}

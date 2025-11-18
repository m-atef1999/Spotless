using FluentValidation;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Validation
{

    public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerDto>
    {
        public RegisterCustomerRequestValidator()
        {


            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9\s-]{7,20}$").WithMessage("Phone number format is invalid.");


            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street address is required.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Zip code is required.")
                .MaximumLength(10).WithMessage("Zip code cannot exceed 10 characters.");
        }
    }
}
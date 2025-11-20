using FluentValidation;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Validation
{
    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage("Service ID is required for every order item.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100 for a single service.");
        }
    }
}

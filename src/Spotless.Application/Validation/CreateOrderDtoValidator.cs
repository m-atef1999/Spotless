using FluentValidation;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Validation
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {

            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage("Service ID is required.")
                .Must(BeAValidGuid).WithMessage("Service ID must be a valid GUID.");


            RuleFor(x => x.PickupTime)
                .NotEmpty().WithMessage("Pickup time is required.")
                .GreaterThan(DateTime.UtcNow.AddMinutes(30))
                    .WithMessage("Pickup time must be at least 30 minutes in the future.");

            RuleFor(x => x.DeliveryTime)
                .NotEmpty().WithMessage("Delivery time is required.")
                .GreaterThan(x => x.PickupTime)
                    .WithMessage("Delivery time must be after the pickup time.");


            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");


            RuleFor(x => x.PickupAddress.Street).NotEmpty().WithMessage("Pickup street is required.");
            RuleFor(x => x.PickupAddress.City).NotEmpty().WithMessage("Pickup city is required.");
        }

        private bool BeAValidGuid(Guid id) => id != Guid.Empty;
    }
}

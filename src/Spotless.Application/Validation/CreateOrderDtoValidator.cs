using FluentValidation;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Validation
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("The order must contain at least one service item.")
                .Must(items => items != null && items.All(item => item.Quantity > 0))
                    .WithMessage("All order items must have a positive quantity.");


            RuleFor(x => x.TimeSlotId)
                .NotEmpty().WithMessage("A time slot ID is required.")
                .Must(BeAValidGuid).WithMessage("Time Slot ID must be a valid GUID.");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required.")
                .GreaterThan(DateTime.Today)
                    .WithMessage("Scheduled date must be today or in the future.");


            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");


            RuleFor(x => x.PickupLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Pickup latitude must be between -90 and 90 degrees.");


            RuleFor(x => x.PickupLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Pickup longitude must be between -180 and 180 degrees.");


            RuleFor(x => x.DeliveryLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Delivery latitude must be between -90 and 90 degrees.");


            RuleFor(x => x.DeliveryLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Delivery longitude must be between -180 and 180 degrees.");


        }

        private bool BeAValidGuid(Guid id) => id != Guid.Empty;
    }
}

using FluentValidation;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Validation
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {

            RuleFor(x => x.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required.")
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Scheduled date cannot be in the past.");

            RuleFor(x => x.TimeSlotId)
                .NotEmpty().WithMessage("Time slot selection is required.");


            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method specified.");



            RuleFor(x => x.PickupLatitude)
                .InclusiveBetween(-90.0M, 90.0M).WithMessage("Pickup latitude must be between -90 and 90.");


            RuleFor(x => x.PickupLongitude)
                .InclusiveBetween(-180.0M, 180.0M).WithMessage("Pickup longitude must be between -180 and 180.");


            RuleFor(x => x.DeliveryLatitude)
                .InclusiveBetween(-90.0M, 90.0M).WithMessage("Delivery latitude must be between -90 and 90.");


            RuleFor(x => x.DeliveryLongitude)
                .InclusiveBetween(-180.0M, 180.0M).WithMessage("Delivery longitude must be between -180 and 180.");



            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("The order must contain at least one item.")
                .Must(items => items != null && items.Any())
                .WithMessage("The items list cannot be empty.");


            RuleForEach(x => x.Items).SetValidator(new CreateOrderItemDtoValidator());
        }
    }
}

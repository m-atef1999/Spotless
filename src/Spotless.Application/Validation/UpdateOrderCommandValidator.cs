using FluentValidation;
using Spotless.Application.Features.Orders;

namespace Spotless.Application.Validation
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {

            RuleFor(command => command.Dto).NotNull().WithMessage("Order update data is required.");
            RuleFor(command => command.OrderId).NotEmpty().WithMessage("Order ID is required.");
            RuleFor(command => command.CustomerId).NotEmpty().WithMessage("Customer ID is required.");



            RuleFor(command => command.Dto.TimeSlotId)
                .NotEmpty().WithMessage("A new Time Slot ID is required for rescheduling.");

            RuleFor(command => command.Dto.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required for rescheduling.")
                .GreaterThanOrEqualTo(DateTime.Today)
                    .WithMessage("Scheduled date must be today or in the future.");





            RuleFor(command => command.Dto.PickupLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Pickup latitude must be between -90 and 90 degrees.");


            RuleFor(command => command.Dto.PickupLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Pickup longitude must be between -180 and 180 degrees.");

            RuleFor(command => command.Dto.DeliveryLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Delivery latitude must be between -90 and 90 degrees.");


            RuleFor(command => command.Dto.DeliveryLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Delivery longitude must be between -180 and 180 degrees.");
        }
    }
}

using FluentValidation;
using Spotless.Application.Features.Orders.Commands.UpdateOrder;

namespace Spotless.Application.Validation
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(command => command.Dto).NotNull().WithMessage("Order update data is required.");


            RuleFor(command => command.Dto.ServiceId)
                .NotEmpty().WithMessage("Service ID must be specified.");


            RuleFor(command => command.Dto.PickupTime)
                .NotEmpty().WithMessage("Pickup time is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Pickup time must be in the future.");

            RuleFor(command => command.Dto.DeliveryTime)
                .NotEmpty().WithMessage("Delivery time is required.")

                .GreaterThan(command => command.Dto.PickupTime)
                .WithMessage("Delivery time must be later than the pickup time.");


            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.CustomerId).NotEmpty();
        }
    }
}

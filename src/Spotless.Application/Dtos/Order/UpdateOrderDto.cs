namespace Spotless.Application.Dtos.Order
{

    public record UpdateOrderDto(

        Guid? ServiceId,


        Guid? TimeSlotId,
        DateTime? ScheduledDate,

        decimal? PickupLatitude,
        decimal? PickupLongitude,

        decimal? DeliveryLatitude,
        decimal? DeliveryLongitude


    );
}

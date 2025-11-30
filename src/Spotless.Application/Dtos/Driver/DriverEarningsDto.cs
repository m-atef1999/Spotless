namespace Spotless.Application.Dtos.Driver
{
    public class DriverEarningsDto
    {
        public double TotalEarnings { get; set; }
        public double PendingPayments { get; set; }
        public int CompletedOrders { get; set; }
        public double AverageRating { get; set; }
    }
}

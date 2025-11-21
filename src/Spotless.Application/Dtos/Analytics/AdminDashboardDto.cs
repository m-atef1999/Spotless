namespace Spotless.Application.Dtos.Analytics
{
    public record AdminDashboardDto
    {
        public int TotalOrders { get; init; }
        public int PendingOrders { get; init; }
        public int CompletedOrders { get; init; }
        public int CancelledOrders { get; init; }
        public int ActiveDrivers { get; init; }
        public int TotalDrivers { get; init; }
        public int TotalCustomers { get; init; }
        public decimal TotalRevenue { get; init; }
        public decimal MonthlyRevenue { get; init; }
        public int TotalServices { get; init; }
        public int TotalCategories { get; init; }
    }
}

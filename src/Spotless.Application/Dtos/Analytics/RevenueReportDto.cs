namespace Spotless.Application.Dtos.Analytics
{
    public record RevenueReportDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal TotalRevenue { get; init; }
        public int TotalOrders { get; init; }
        public decimal AverageOrderValue { get; init; }
        public List<DailyRevenueDto> DailyBreakdown { get; init; } = new();
    }

    public record DailyRevenueDto
    {
        public DateTime Date { get; init; }
        public decimal Revenue { get; init; }
        public int OrderCount { get; init; }
    }
}

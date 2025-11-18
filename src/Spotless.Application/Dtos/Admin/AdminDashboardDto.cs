namespace Spotless.Application.Dtos.Admin
{
    public record MostUsedServiceDto(
    Guid ServiceId,
    string ServiceName,
    int OrderCount
);

    public record AdminDashboardDto(
        int TotalOrdersToday,
        decimal RevenueToday,
        string RevenueCurrency,
        IReadOnlyList<MostUsedServiceDto> MostUsedServices,
        int NumberOfActiveCleaners,
        int NewRegistrationsToday,
        int PendingBookings
    );
}


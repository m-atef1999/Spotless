using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Driver
{
    public record DriverApplicationDto
    (
        Guid Id,
        Guid DriverId,
        string DriverName,
        OrderDriverApplicationStatus Status,
        DateTime AppliedAt
    );
}
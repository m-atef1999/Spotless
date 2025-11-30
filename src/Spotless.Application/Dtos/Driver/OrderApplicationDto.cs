using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Driver
{
    public record OrderApplicationDto
    (
        Guid Id,
        Guid DriverId,
        string DriverName,
        OrderDriverApplicationStatus Status,
        DateTime AppliedAt
    );
}

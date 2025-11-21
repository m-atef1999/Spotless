using MediatR;
using Spotless.Application.Dtos.Settings;

namespace Spotless.Application.Features.Settings.Queries.ListSettings
{
    public record ListSettingsQuery(string? Category = null) : IRequest<IReadOnlyList<SystemSettingDto>>;
}

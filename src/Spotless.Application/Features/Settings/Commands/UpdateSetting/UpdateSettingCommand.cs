using MediatR;
using Spotless.Application.Dtos.Settings;

namespace Spotless.Application.Features.Settings.Commands.UpdateSetting
{
    public record UpdateSettingCommand(Guid Id, UpdateSystemSettingDto Dto) : IRequest<Unit>;
}

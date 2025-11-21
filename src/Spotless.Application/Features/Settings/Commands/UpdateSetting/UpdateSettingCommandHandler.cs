using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Settings.Commands.UpdateSetting
{
    public class UpdateSettingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateSettingCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
        {
            var setting = await _unitOfWork.SystemSettings.GetByIdAsync(request.Id)
                ?? throw new KeyNotFoundException($"System setting with ID {request.Id} not found");

            setting.UpdateValue(request.Dto.Value);
            await _unitOfWork.SystemSettings.UpdateAsync(setting);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

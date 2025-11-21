using MediatR;
using Spotless.Application.Dtos.Settings;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Settings.Queries.ListSettings
{
    public class ListSettingsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<ListSettingsQuery, IReadOnlyList<SystemSettingDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<SystemSettingDto>> Handle(ListSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = string.IsNullOrEmpty(request.Category)
                ? await _unitOfWork.SystemSettings.GetAllAsync()
                : await _unitOfWork.SystemSettings.GetAsync(s => s.Category == request.Category);

            return settings
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Key)
                .Select(s => new SystemSettingDto
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value,
                    Category = s.Category,
                    Description = s.Description,
                    LastModified = s.LastModified
                })
                .ToList();
        }
    }
}

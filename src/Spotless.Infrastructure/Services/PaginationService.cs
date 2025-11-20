using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    public class PaginationService(IOptions<PaginationSettings> options) : IPaginationService
    {
        private readonly PaginationSettings _settings = options.Value;

        public int NormalizePageSize(int? requestedPageSize)
        {
            if (!requestedPageSize.HasValue || requestedPageSize <= 0)
                return _settings.DefaultPageSize;

            return Math.Min(requestedPageSize.Value, _settings.MaxPageSize);
        }

        public int GetDefaultPageNumber()
        {
            return _settings.DefaultPageNumber;
        }
    }
}

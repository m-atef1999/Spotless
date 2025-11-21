using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class SystemSettingRepository(ApplicationDbContext context) : BaseRepository<SystemSetting>(context), ISystemSettingRepository
    {
    }
}

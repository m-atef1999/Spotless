using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class AuditLogRepository(ApplicationDbContext context) : BaseRepository<AuditLog>(context), IAuditLogRepository
    {
    }
}

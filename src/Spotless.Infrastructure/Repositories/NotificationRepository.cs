using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class NotificationRepository(ApplicationDbContext context) : BaseRepository<Notification>(context), INotificationRepository
    {
    }
}

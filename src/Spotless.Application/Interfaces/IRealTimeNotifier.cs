using System;
using System.Threading.Tasks;

namespace Spotless.Application.Interfaces
{
    public interface IRealTimeNotifier
    {
        Task NotifyDriverAssignedAsync(Guid driverId, Guid orderId);
    }
}

using Spotless.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Spotless.Application.Interfaces
{
    public interface IRouterService
    {
        /// <summary>
        /// Calculates estimated travel time (ETA) between two locations.
        /// </summary>
        /// <returns>Nullable TimeSpan if ETA cannot be calculated.</returns>
        Task<TimeSpan?> CalculateEtaAsync(Location from, Location to);
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Spotless.Infrastructure.Services
{
    public class FallbackRouterService(IOptions<RoutingSettings> settings, ILogger<FallbackRouterService> logger) : IRouterService
    {
        private readonly RoutingSettings _settings = settings.Value;
        private readonly ILogger<FallbackRouterService> _logger = logger;

        public Task<TimeSpan?> CalculateEtaAsync(Location from, Location to)
        {
            if (from == null || to == null || !from.Latitude.HasValue || !from.Longitude.HasValue || !to.Latitude.HasValue || !to.Longitude.HasValue)
                return Task.FromResult<TimeSpan?>(null);

            // Haversine distance in kilometers
            double lat1 = (double)from.Latitude.Value;
            double lon1 = (double)from.Longitude.Value;
            double lat2 = (double)to.Latitude.Value;
            double lon2 = (double)to.Longitude.Value;

            double R = 6371; // km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distanceKm = R * c;

            var speedKph = _settings.DefaultAverageSpeedKph > 0 ? _settings.DefaultAverageSpeedKph : 30.0;
            var hours = distanceKm / speedKph;
            var seconds = hours * 3600;

            _logger.LogInformation("Fallback ETA estimated: {DistanceKm} km, speed {SpeedKph} kph, seconds {Seconds}", distanceKm, speedKph, seconds);

            return Task.FromResult<TimeSpan?>(TimeSpan.FromSeconds(seconds));
        }

        private static double ToRadians(double angle) => (Math.PI / 180) * angle;
    }
}

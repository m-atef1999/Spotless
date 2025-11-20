using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Spotless.Infrastructure.Services
{
    public class MapboxRouterService(HttpClient http, IOptions<RoutingSettings> settings, ILogger<MapboxRouterService> logger) : IRouterService
    {
        private readonly HttpClient _http = http;
        private readonly RoutingSettings _settings = settings.Value;
        private readonly ILogger<MapboxRouterService> _logger = logger;

        public async Task<TimeSpan?> CalculateEtaAsync(Location from, Location to)
        {
            if (from == null || to == null || !from.Latitude.HasValue || !from.Longitude.HasValue || !to.Latitude.HasValue || !to.Longitude.HasValue)
                return null;

            if (string.IsNullOrEmpty(_settings.MapboxApiKey) || _settings.MapboxApiKey.Contains("PLACEHOLDER") || _settings.MapboxApiKey.StartsWith("pk.test"))
            {
                _logger.LogWarning("Mapbox API key not configured or placeholder detected, cannot calculate ETA via Mapbox");
                return null;
            }

            try
            {
                // Mapbox expects lon,lat
                var coordinates = $"{from.Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{from.Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)};{to.Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{to.Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                var url = $"https://api.mapbox.com/directions/v5/mapbox/driving/{coordinates}?access_token={_settings.MapboxApiKey}&overview=false&annotations=duration";

                var resp = await _http.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Mapbox request failed with status {Status}", resp.StatusCode);
                    return null;
                }

                using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.TryGetProperty("routes", out var routes) && routes.GetArrayLength() > 0)
                {
                    var first = routes[0];
                    if (first.TryGetProperty("duration", out var durationProp) && durationProp.TryGetDouble(out var durationSeconds))
                    {
                        return TimeSpan.FromSeconds(durationSeconds);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mapbox ETA calculation failed");
                return null;
            }
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Spotless.API.Hubs;
using Spotless.Application.Interfaces;
using Spotless.Application.Configurations;
using Spotless.Domain.ValueObjects;
using Spotless.Infrastructure.Context;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Spotless.API.Services
{
    public class SignalRRealTimeNotifier(IHubContext<DriverHub> hubContext, IUnitOfWork unitOfWork, IRouterService routerService, ILogger<SignalRRealTimeNotifier> logger) : IRealTimeNotifier
    {
        private readonly IHubContext<DriverHub> _hubContext = hubContext;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRouterService _routerService = routerService;
        private readonly ILogger<SignalRRealTimeNotifier> _logger = logger;

        public async Task NotifyDriverAssignedAsync(Guid driverId, Guid orderId)
        {
            var groupName = GetDriverGroup(driverId);
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) return;

                // Try to compute ETA using configured router service
                string etaText = "15-30 mins";
                try
                {
                    var eta = await _routerService.CalculateEtaAsync(order.PickupLocation, order.DeliveryLocation);
                    if (eta.HasValue)
                    {
                        var mins = Math.Ceiling(eta.Value.TotalMinutes);
                        etaText = $"~{mins} mins";
                    }
                }
                catch (Exception rex)
                {
                    _logger.LogWarning(rex, "Router service failed to calculate ETA for order {OrderId}", orderId);
                }

                var payload = new
                {
                    OrderId = orderId.ToString(),
                    PickupAddress = order.PickupLocation?.ToString() ?? "Unknown",
                    DeliveryAddress = order.DeliveryLocation?.ToString() ?? "Unknown",
                    ETA = etaText,
                    Status = "Assigned"
                };

                await _hubContext.Clients.Group(groupName).SendAsync("DriverAssigned", payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR notification failed for driver {DriverId} order {OrderId}", driverId, orderId);
            }
        }

        private static string GetDriverGroup(Guid driverId) => $"driver:{driverId}";
    }
}

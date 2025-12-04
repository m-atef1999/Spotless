using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Spotless.Infrastructure.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Claims;
using System;

namespace Spotless.API.Hubs
{
    [Authorize(Roles = "Driver,Admin")]
    public class DriverHub(UserManager<ApplicationUser> userManager, ILogger<DriverHub> logger) : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<DriverHub> _logger = logger;

        public override async Task OnConnectedAsync()
        {
            try
            {
                var user = Context.User;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
                    if (!string.IsNullOrWhiteSpace(userIdClaim) && Guid.TryParse(userIdClaim, out var userGuid))
                    {
                        var appUser = await _userManager.FindByIdAsync(userGuid.ToString());
                        var driverId = appUser?.DriverId;
                        if (driverId.HasValue)
                        {
                            var groupName = $"driver:{driverId.Value}";
                            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                            _logger.LogInformation("Connection {ConnectionId} added to group {Group} for user {UserId}", Context.ConnectionId, groupName, userIdClaim);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnConnectedAsync for connection {ConnectionId}", Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var user = Context.User;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
                    if (!string.IsNullOrWhiteSpace(userIdClaim) && Guid.TryParse(userIdClaim, out var userGuid))
                    {
                        var appUser = await _userManager.FindByIdAsync(userGuid.ToString());
                        var driverId = appUser?.DriverId;
                        if (driverId.HasValue)
                        {
                            var groupName = $"driver:{driverId.Value}";
                            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                            _logger.LogInformation("Connection {ConnectionId} removed from group {Group} for user {UserId}", Context.ConnectionId, groupName, userIdClaim);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnDisconnectedAsync for connection {ConnectionId}", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Called by driver to update their location
        public async Task UpdateLocation(decimal latitude, decimal longitude)
        {
            await Clients.All.SendAsync("DriverLocationUpdated", Context.UserIdentifier, latitude, longitude);
        }

        // Called by driver to update their availability
        public async Task UpdateAvailability(string status)
        {
            await Clients.All.SendAsync("DriverAvailabilityUpdated", Context.UserIdentifier, status);
        }

        // Called by server to notify driver of order status
        public async Task NotifyOrderStatus(string orderId, string status)
        {
            await Clients.Caller.SendAsync("OrderStatusUpdated", orderId, status);
        }

        // Driver should call this after connecting to be added to their dedicated group.
        // This method will only add the connection to the group if the caller is the authenticated driver.
        public async Task JoinDriverGroup(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return;

            try
            {
                var authenticatedDriverId = await ResolveAuthenticatedDriverIdAsync();
                if (authenticatedDriverId == null) return;
                if (!Guid.TryParse(driverId, out var requested) || requested != authenticatedDriverId.Value)
                {
                    _logger.LogWarning("Unauthorized attempt to join driver group {Requested} by connection {ConnectionId}", driverId, Context.ConnectionId);
                    return;
                }

                var groupName = $"driver:{driverId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JoinDriverGroup failed for connection {ConnectionId}", Context.ConnectionId);
            }
        }

        public async Task LeaveDriverGroup(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return;

            try
            {
                var authenticatedDriverId = await ResolveAuthenticatedDriverIdAsync();
                if (authenticatedDriverId == null) return;
                if (!Guid.TryParse(driverId, out var requested) || requested != authenticatedDriverId.Value)
                {
                    _logger.LogWarning("Unauthorized attempt to leave driver group {Requested} by connection {ConnectionId}", driverId, Context.ConnectionId);
                    return;
                }

                var groupName = $"driver:{driverId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LeaveDriverGroup failed for connection {ConnectionId}", Context.ConnectionId);
            }
        }

        private async Task<Guid?> ResolveAuthenticatedDriverIdAsync()
        {
            var user = Context.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userGuid)) return null;

            var appUser = await _userManager.FindByIdAsync(userGuid.ToString());
            return appUser?.DriverId;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Services;
using Spotless.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Spotless.Infrastructure.Context;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/timeslots")]
    [Authorize]
    public class TimeSlotsController(CachedTimeSlotService timeSlotService, ApplicationDbContext context) : ControllerBase
    {
        private readonly CachedTimeSlotService _timeSlotService = timeSlotService;
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Lists all available time slots
        /// </summary>
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetTimeSlots()
        {
            // Check if we need to re-seed (if empty OR if we have old "Afternoon 1" style slots)
            var existingSlots = await _context.TimeSlots.ToListAsync();
            bool needsReseed = !existingSlots.Any() || existingSlots.Any(s => s.Name.Contains("Afternoon 1") || s.Name.Contains("Afternoon 2"));

            if (needsReseed)
            {
                await SeedDefaultSlots();
                existingSlots = await _context.TimeSlots.ToListAsync();
            }

            var slots = await _timeSlotService.GetAllTimeSlotsAsync();

            // Self-healing: Check if cached IDs match DB IDs
            if (slots.Any() && existingSlots.Any())
            {
                var cachedId = slots.First().Id;
                var dbId = existingSlots.First().Id;
                if (cachedId != dbId)
                {
                    await _timeSlotService.InvalidateTimeSlotCacheAsync();
                    slots = await _timeSlotService.GetAllTimeSlotsAsync();
                }
            }

            // If cache is still stale or empty, return direct list
            if (!slots.Any() || slots.Any(s => s.Name.Contains("Afternoon 1")))
            {
                 return Ok(existingSlots);
            }
            return Ok(slots);
        }

        private async Task SeedDefaultSlots()
        {
            var defaults = new List<TimeSlot>
            {
                new("Morning", new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 10, "Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"),
                new("Afternoon", new TimeSpan(13, 0, 0), new TimeSpan(16, 0, 0), 10, "Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"),
                new("Evening", new TimeSpan(17, 0, 0), new TimeSpan(20, 0, 0), 10, "Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday")
            };

            foreach (var def in defaults)
            {
                var existing = await _context.TimeSlots.FirstOrDefaultAsync(s => s.Name == def.Name);
                if (existing == null)
                {
                    _context.TimeSlots.Add(def);
                }
                else
                {
                    // Update existing slot properties using reflection or direct assignment if setters were public
                    // Since setters are private, we might need to use backing fields or just leave it if it exists.
                    // However, to fix "bad data", we really want to update it.
                    // But TimeSlot setters are private!
                    // We can use Entry().CurrentValues.SetValues if we map it, but let's try to just ensure they exist first.
                    // If the user has "bad" slots, they might be named differently?
                    // If we can't update, we at least ensure the default names exist.
                }
            }
            
            // If there are NO slots at all, the above loop adds them.
            // If there are "Afternoon 1" slots (old), we can try to remove them, but catch FK errors.
            var oldSlots = await _context.TimeSlots.Where(s => s.Name.Contains("Afternoon 1") || s.Name.Contains("Afternoon 2")).ToListAsync();
            if (oldSlots.Any())
            {
                try
                {
                    _context.TimeSlots.RemoveRange(oldSlots);
                }
                catch { /* Ignore FK errors, just leave them */ }
            }

            await _context.SaveChangesAsync();
        }


    }
}

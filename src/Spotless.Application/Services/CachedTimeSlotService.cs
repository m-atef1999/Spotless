using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;

namespace Spotless.Application.Services
{
    public class CachedTimeSlotService(IRepository<TimeSlot> timeSlotRepository, ICachingService cachingService)
    {
        private readonly IRepository<TimeSlot> _timeSlotRepository = timeSlotRepository;
        private readonly ICachingService _cachingService = cachingService;
        private const string TIMESLOTS_CACHE_KEY = "timeslots:all";

        public async Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<TimeSlot>>(TIMESLOTS_CACHE_KEY);
            if (cached != null) return cached;

            var timeSlots = await _timeSlotRepository.GetAllAsync();
            await _cachingService.SetAsync(TIMESLOTS_CACHE_KEY, timeSlots, TimeSpan.FromHours(12));
            return timeSlots;
        }

        public async Task InvalidateTimeSlotCacheAsync()
        {
            await _cachingService.RemoveAsync(TIMESLOTS_CACHE_KEY);
        }
    }
}

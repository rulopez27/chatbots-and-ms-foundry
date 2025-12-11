using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roboto.Service.Services;

namespace Roboto.Service.Controllers
{
    /// <summary>
    /// Handles user-specific calendar operations
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/calendar-events")]
    public class UserCalendarController : ControllerBase
    {
        private readonly ICalendarEventService _calendarEventService;

        public UserCalendarController(ICalendarEventService calendarEventService)
        {
            _calendarEventService = calendarEventService;
        }

        /// <summary>
        /// Get all calendar events for a specific user
        /// GET /api/users/{userId}/calendar-events
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserEvents(int userId)
        {
            try
            {
                var eventDtos = await _calendarEventService.GetEventsByUserIdAsync(userId);
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }

        /// <summary>
        /// Get calendar events within a date range for a specific user
        /// GET /api/users/{userId}/calendar-events?startDate=2025-01-01&endDate=2025-12-31
        /// </summary>
        [HttpGet("range")]
        public async Task<IActionResult> GetEventsInDateRange(
            int userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var eventDtos = await _calendarEventService.GetEventsInDateRangeAsync(userId, startDate, endDate);
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }

        /// <summary>
        /// Get calendar conflicts within a date range for a specific user
        /// GET /api/users/{userId}/calendar-events/conflicts?startDate=2025-01-01&endDate=2025-12-31
        /// </summary>
        [HttpGet("conflicts")]
        public async Task<IActionResult> GetCalendarConflicts(
            int userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var conflictDtos = await _calendarEventService.GetCalendarConflictsAsync(userId, startDate, endDate);
                return Ok(conflictDtos);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }
    }
}

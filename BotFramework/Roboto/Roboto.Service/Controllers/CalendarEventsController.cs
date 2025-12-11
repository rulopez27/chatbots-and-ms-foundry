using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roboto.Dtos;
using Roboto.Service.Services;

namespace Roboto.Service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarEventsController : Controller
    {
        private readonly ICalendarEventService _calendarEventService;

        public CalendarEventsController(ICalendarEventService calendarEventService)
        {
            _calendarEventService = calendarEventService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CalendarEventCreateDto dto)
        {
            try
            {
                var eventDto = await _calendarEventService.CreateEventAsync(dto);
                return Created($"/api/calendarevents/{eventDto.Id}", eventDto);
            }
            catch(Exception ex)
            {
                string errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return StatusCode(500, errorMessage);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var eventDto = await _calendarEventService.GetEventByIdAsync(id);
                if (eventDto == null)
                {
                    return NotFound();
                }

                return Ok(eventDto);
            }
            catch(Exception ex)
            {
                string errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return StatusCode(500, errorMessage);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CalendarEventDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest("Route ID and DTO ID must match");
                }

                var updatedEvent = await _calendarEventService.UpdateEventAsync(dto);
                if (updatedEvent == null)
                {
                    return NotFound();
                }

                return Ok(updatedEvent);
            }
            catch(Exception ex)
            {
                string errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return StatusCode(500, errorMessage);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _calendarEventService.DeleteEventAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch(Exception ex)
            {
                string errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return StatusCode(500, errorMessage);
            }
        }     
    }  
}
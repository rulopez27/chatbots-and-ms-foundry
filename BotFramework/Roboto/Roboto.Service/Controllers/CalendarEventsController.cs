using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roboto.Models;
using Roboto.Repository;
using Roboto.Service.Dto;

namespace Roboto.Service.Controllers
{
    [Authorize]
    [Route("api/CalendarEvents")]
    public class CalendarEventsController : Controller
    {
        private readonly ICalendarEventRepository _repository;

        public CalendarEventsController(ICalendarEventRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CalendarEventCreateDto dto)
        {
            if(string.IsNullOrEmpty(dto.Title) ||
                (dto.StartDateTime == DateTime.MinValue || dto.StartDateTime < DateTime.MinValue) ||
                dto.Duration == 0)
            {
                return BadRequest("Event title, start date and time or duration are invalid.");
            }
            
            CalendarEvent calendarEvent = CalendarEventConverter.ToEntity(dto);
            await _repository.AddEventAsync(calendarEvent);
            return Created(nameof(CalendarEvent),CalendarEventConverter.ToDto(calendarEvent));
        }
    }
}
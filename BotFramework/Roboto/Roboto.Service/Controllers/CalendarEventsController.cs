using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roboto.Models;
using Roboto.Repository;
using Roboto.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Roboto.Service.Services;

namespace Roboto.Service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarEventsController : Controller
    {
        private readonly ICalendarEventRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILinkService _linkService;
        private readonly LinkGenerator _linkGenerator;

        public CalendarEventsController(ICalendarEventRepository repository, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            ILinkService linkService,
            LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _linkService = linkService;
            _linkGenerator = linkGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CalendarEventCreateDto dto)
        {
            try
            {
                if(string.IsNullOrEmpty(dto.Title) || dto.Duration == 0)
                {
                    return BadRequest("Event title, start date and time or duration are invalid.");
                }
                
                CalendarEvent calendarEvent = _mapper.Map<CalendarEvent>(dto);
                await _repository.AddEventAsync(calendarEvent);
                CalendarEventDto returnDto = _mapper.Map<CalendarEventDto>(calendarEvent);
                returnDto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
                return Created(nameof(CalendarEvent),returnDto);
            }
            catch(Exception ex)
            {
                string errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return StatusCode(500, errorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                CalendarEvent calendarEvent = await _repository.GetEventByIdAsync(id);
                if(calendarEvent == null)
                {
                    return NotFound();
                }

                CalendarEventDto calendarEventDto = _mapper.Map<CalendarEventDto>(calendarEvent);
                calendarEventDto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
                return Ok(calendarEventDto);   
            }
            catch(Exception ex)
            {
                string errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return StatusCode(500, errorMessage);
            }
            
        }
    }
}
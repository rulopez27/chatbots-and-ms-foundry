using AutoMapper;
using Roboto.Dtos;
using Roboto.Models;
using Roboto.Repository;
using Roboto.Service.Extensions;

namespace Roboto.Service.Services
{
    public class CalendarEventService : ICalendarEventService
    {
        private readonly ICalendarEventRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILinkService _linkService;
        private readonly LinkGenerator _linkGenerator;

        public CalendarEventService(
            ICalendarEventRepository repository,
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

        public async Task<CalendarEventDto> CreateEventAsync(CalendarEventCreateDto dto)
        {
            var calendarEvent = new CalendarEvent();
            _mapper.Map(dto, calendarEvent);
            await _repository.AddEventAsync(calendarEvent);
            
            var returnDto = _mapper.Map<CalendarEventDto>(calendarEvent);
            returnDto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
            
            return returnDto;
        }

        public async Task<CalendarEventDto?> GetEventByIdAsync(int id)
        {
            var calendarEvent = await _repository.GetEventByIdAsync(id);
            if (calendarEvent == null)
            {
                return null;
            }

            var dto = new CalendarEventDto();
            _mapper.Map(calendarEvent, dto);
            dto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
            
            return dto;
        }

        public async Task<CalendarEventDto?> UpdateEventAsync(CalendarEventDto dto)
        {
            var existingEvent = await _repository.GetEventByIdAsync(dto.Id);
            if (existingEvent == null)
            {
                return null;
            }

            _mapper.Map(dto, existingEvent);
            await _repository.UpdateEventAsync(existingEvent);
            
            var returnDto = _mapper.Map<CalendarEventDto>(existingEvent);
            returnDto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
            
            return returnDto;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var existingEvent = await _repository.GetEventByIdAsync(id);
            if (existingEvent == null)
            {
                return false;
            }

            await _repository.DeleteEventAsync(id);
            return true;
        }

        public async Task<IEnumerable<CalendarEventDto>> GetEventsByUserIdAsync(int userId)
        {
            var events = await _repository.GetEventsByUserIdAsync(userId);
            return MapEventsToDto(events);
        }

        public async Task<IEnumerable<CalendarEventDto>> GetEventsInDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var events = await _repository.GetEventsInDateRangeAsync(userId, startDate, endDate);
            return MapEventsToDto(events);
        }

        public async Task<IEnumerable<CalendarEventDto>> GetCalendarConflictsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var conflicts = await _repository.GetCalendarConflictsAsync(userId, startDate, endDate);
            return MapEventsToDto(conflicts);
        }

        private IEnumerable<CalendarEventDto> MapEventsToDto(IEnumerable<CalendarEvent> events)
        {
            return events.Select(ev =>
            {
                var dto = new CalendarEventDto();
                _mapper.Map(ev, dto);
                dto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
                return dto;
            }).ToList();
        }
    }
}

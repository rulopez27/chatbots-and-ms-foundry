using System.Net.Http.Json;
using Roboto.Sdk.Exceptions;
using Roboto.Dtos;

namespace Roboto.Sdk.Services
{
    internal class CalendarEventService : ICalendarEventService
    {
        private readonly HttpClient _httpClient;

        public CalendarEventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CalendarEventDto> CreateEventAsync(CalendarEventCreateDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/calendarevents", dto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to create event: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<CalendarEventDto>(cancellationToken))!;
        }

        public async Task<CalendarEventDto> GetEventByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/calendarevents/{id}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new RobotoNotFoundException($"Calendar event with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to get event: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<CalendarEventDto>(cancellationToken))!;
        }

        public async Task<CalendarEventDto> UpdateEventAsync(CalendarEventDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/calendarevents/{dto.Id}", dto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to update event: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<CalendarEventDto>(cancellationToken))!;
        }

        public async Task DeleteEventAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/calendarevents/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to delete event: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }
        }

        public async Task<IEnumerable<CalendarEventDto>> GetEventsForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}/calendar-events", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to get events: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<IEnumerable<CalendarEventDto>>(cancellationToken))!;
        }

        public async Task<IEnumerable<CalendarEventDto>> GetEventsInDateRangeAsync(int userId, CalendarEventsRangeDto dateRange, CancellationToken cancellationToken = default)
        {
            var startDate = dateRange.StartDate.ToString("yyyy-MM-dd");
            var endDate = dateRange.EndDate.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"api/users/{userId}/calendar-events/range?startDate={startDate}&endDate={endDate}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to get events in range: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<IEnumerable<CalendarEventDto>>(cancellationToken))!;
        }

        public async Task<IEnumerable<CalendarEventDto>> GetCalendarConflictsAsync(int userId, CalendarEventsRangeDto dateRange, CancellationToken cancellationToken = default)
        {
            var startDate = dateRange.StartDate.ToString("yyyy-MM-dd");
            var endDate = dateRange.EndDate.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"api/users/{userId}/calendar-events/conflicts?startDate={startDate}&endDate={endDate}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to get calendar conflicts: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<IEnumerable<CalendarEventDto>>(cancellationToken))!;
        }
    }
}

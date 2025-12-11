using Roboto.Dtos;

namespace Roboto.Service.Extensions
{
    public static class CalendarEventExtensions
    {
        public static void CreateLinks(this CalendarEventDto calendarEvent, ILinkService linkService, LinkGenerator linkGenerator, IHttpContextAccessor context)
        {
            calendarEvent.Links.Add(linkService.Generate("Get", "CalendarEvents", new {id = calendarEvent.Id}, "self", "GET"));
            calendarEvent.Links.Add(linkService.Generate("Put", "CalendarEvents", null, "update", "PUT"));
            calendarEvent.Links.Add(linkService.Generate("Delete", "CalendarEvents", new {id = calendarEvent.Id}, "delete", "DELETE"));
        }
    }   
}
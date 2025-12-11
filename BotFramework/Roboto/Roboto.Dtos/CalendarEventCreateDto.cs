namespace Roboto.Dtos
{
    public class CalendarEventCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public double Duration { get; set; }
        public bool IsAllDay { get; set; }
        public string Details { get; set; } = string.Empty;
        public bool BlockCalendar { get; set; }
        public int UserId { get; set; }

    }
}
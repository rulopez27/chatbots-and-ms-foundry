namespace Roboto.Models.Dto
{
    public class CalendarEventDto : DtoBase
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double Duration { get; set; }
        public bool IsAllDay { get; set; }
        public string Details { get; set; } = string.Empty;
        public bool BlockCalendar { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
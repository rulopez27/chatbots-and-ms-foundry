namespace Roboto.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public double Duration { get; set; }
        public bool IsAllDay { get; set; }
        public string? Details { get; set; }
        public DateTime EndDateTime {get; set;}
        public bool BlockCalendar { get; set; }
        public virtual User? User { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public CalendarEvent()
        {
            Title = string.Empty;
            Details = string.Empty;
            CreatedAt = DateTime.Now;
            ModifiedAt = DateTime.MinValue;
        }

        public CalendarEvent(int userId, string title, DateTime startDateTime, double duration, bool isAllDay, bool blockCalendar, string details)
        {
            UserId = userId;
            Title = title;
            StartDateTime = startDateTime;
            Duration = duration;
            IsAllDay = isAllDay;
            BlockCalendar = blockCalendar;
            Details = details;
            CreatedAt = DateTime.Now;
            ModifiedAt = DateTime.MinValue;
            CalculateEndDateTime();
        }

        public CalendarEvent(int userId, string title, DateTime startDate, string startTime, double duration, bool isAllDay, bool blockCalendar, string details)
        {
            UserId = userId;
            Title = title;
            DateTime.TryParse(startTime, out DateTime startTimeDT);
            StartDateTime = startDate
                .AddHours(startTimeDT.Hour)
                .AddMinutes(startTimeDT.Minute);
            Duration = duration;
            IsAllDay = isAllDay;
            BlockCalendar = blockCalendar;
            Details = details;
            CalculateEndDateTime();
        }

        public override string ToString()
        {
            return $"{Title}: {StartDateTime} - {EndDateTime}";
        }

        public void CalculateEndDateTime()
        {
            EndDateTime = Duration > 0 ? StartDateTime.AddHours(Duration) 
            : StartDateTime.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
    }
}

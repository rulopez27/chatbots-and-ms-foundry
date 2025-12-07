namespace Roboto.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public double Duration { get; set; }
        public bool IsAllDay { get; set; }
        public string Details { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool BlockCalendar { get; set; }
        public virtual User? User { get; set; }
        public int UserId { get; set; }

        public CalendarEvent()
        {
            Title = string.Empty;
            Details = string.Empty;
            
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
            CalculateEndTime();
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
            CalculateEndTime();
        }

        private void CalculateEndTime()
        {
            EndDateTime = IsAllDay ? StartDateTime
                .Date
                .AddHours(23)
                .AddMinutes(59) : StartDateTime.AddHours(Duration);
        }

        public override string ToString()
        {
            return $"{Title}: {StartDateTime} - {EndDateTime}";
        }
    }
}

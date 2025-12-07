namespace Roboto.Models.Dto
{
    public class UserDto : DtoBase
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    }
}
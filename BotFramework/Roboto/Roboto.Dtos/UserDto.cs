namespace Roboto.Dtos
{
    public class UserDto : DtoBase
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public List<CalendarEventDto> CalendarEvents { get; set; } = new List<CalendarEventDto>();

    }
}
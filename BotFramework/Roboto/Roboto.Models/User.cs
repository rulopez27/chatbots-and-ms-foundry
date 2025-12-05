using System;

namespace Roboto.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }

        public User()
        {
            Username = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            Salt = string.Empty;
            CreatedAt = DateTime.MinValue;
            CalendarEvents = new List<CalendarEvent>();
        }
        public User(string username, string email, string passwordHash, string salt)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Salt = salt;
            CreatedAt = DateTime.MinValue;
            CalendarEvents = new List<CalendarEvent>();
        }
    }
}
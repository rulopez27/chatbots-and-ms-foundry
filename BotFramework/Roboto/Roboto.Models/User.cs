using System;
using System.ComponentModel;

namespace Roboto.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }

        public User()
        {
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
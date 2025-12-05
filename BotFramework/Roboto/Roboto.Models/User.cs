using System;

namespace Roboto.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public User()
        {
            
        }
        public User(string username, string email, string passwordHash, string salt)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Salt = salt;
        }
    }
}
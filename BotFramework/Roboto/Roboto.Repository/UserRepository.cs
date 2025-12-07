using Microsoft.EntityFrameworkCore;
using Roboto.Models;

namespace Roboto.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RobotoCalendarSchedulerDbContext _context;

        public UserRepository(RobotoCalendarSchedulerDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UsernameOrEmailExistsAsync(string username, string email)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error checking existence of {username} or {email}", ex);
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
            }
            catch(Exception ex)
            {
                throw new Exception("Error adding new user", ex);
            }

        }

        public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error retrieving user with username {usernameOrEmail}", ex);
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == id );
            }
            catch(Exception ex)
            {
                throw new Exception($"Error retrieving user with id {id}", ex);
            }
        }

        public Task<User?> GetUserByIdWithEventsAsync(int id)
        {
            try
            {
                return _context.Users
                    .Include(u => u.CalendarEvents)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error retrieving user with id {id} including events", ex);
            }
        }
    }
}
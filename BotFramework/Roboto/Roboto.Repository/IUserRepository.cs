using Roboto.Models;

namespace Roboto.Repository
{
    public interface IUserRepository
    {
        Task<bool> UsernameOrEmailExistsAsync(string username, string email);
        Task AddUserAsync(User user);
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByIdWithEventsAsync(int id);
    }
}
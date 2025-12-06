using Roboto.Models;

namespace Roboto.Repository
{
    public interface IUserRepository
    {
        Task<bool> UsernameExistsAsync(string username);
        Task AddUserAsync(User user);
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
    }
}
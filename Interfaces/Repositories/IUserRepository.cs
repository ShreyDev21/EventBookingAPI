using EventBookingAPI.Models;

namespace EventBookingAPI.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}

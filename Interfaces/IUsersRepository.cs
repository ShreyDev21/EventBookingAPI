using EventBookingAPI.DBContext;

namespace EventBookingAPI.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task Delete(User user);
    }
}

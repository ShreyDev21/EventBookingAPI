using EventBookingAPI.DBContext;
using EventBookingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventBookingAPI.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _context;

        public UsersRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get user by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Get user by Id
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Add new user
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        // Delete user
        public Task Delete(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        // Commit changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

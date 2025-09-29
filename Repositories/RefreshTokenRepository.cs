using EventBookingAPI.DBContext;
using EventBookingAPI.Interfaces;
using EventBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBookingAPI.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> Create(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken == null) return false;

            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByUserIdAsync(int userId)
        {
            var tokens = _context.RefreshTokens.Where(r => r.UserId == userId);
            if (!tokens.Any()) return false;

            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

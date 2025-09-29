using EventBookingAPI.DBContext;

namespace EventBookingAPI.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> Create(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);
        Task<bool> DeleteAsync(string token);
        Task<bool> DeleteByUserIdAsync(int userId);
    }
}

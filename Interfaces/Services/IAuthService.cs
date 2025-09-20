using EventBookingAPI.DTOs;

namespace EventBookingAPI.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);          // returns message "Check email for OTP"
        Task<bool> VerifyEmailAsync(VerifyEmailDto dto);      // true if OTP correct
        Task<string?> LoginAsync(LoginDto dto);
    }
}

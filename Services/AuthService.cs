using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Repositories;
using EventBookingAPI.Interfaces.Services;
using EventBookingAPI.Models;

namespace EventBookingAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            // check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email!);
            if (existingUser != null)
                return "Email already registered.";

            // hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(10);

            var newUser = new User
            {
                Username = dto.UserName!,
                Email = dto.Email!,
                PasswordHash = passwordHash,
                Otp = otp,
                OtpExpiry = expiry,
                IsVerified = false,
                Role = "User"
            };

            await _userRepository.AddUserAsync(newUser);
            await _userRepository.SaveChangesAsync();

            // TODO: Send OTP via email (MailKit or SMTP)

            return "Registration successful. Please check your email for OTP.";
        }

        public async Task<bool> VerifyEmailAsync(VerifyEmailDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email!);
            if (user == null) return false;

            if (user.Otp == dto.Otp && user.OtpExpiry > DateTime.UtcNow)
            {
                user.IsVerified = true;
                user.Otp = null;
                user.OtpExpiry = null;

                await _userRepository.UpdateUserAsync(user);
                return await _userRepository.SaveChangesAsync();
            }

            return false;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email!);
            if (user == null || !user.IsVerified) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid) return null;

            // TODO: Generate JWT token (we’ll implement next)
            return "JWT_TOKEN_PLACEHOLDER";
        }
    }
}

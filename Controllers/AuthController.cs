using EventBookingAPI.DBContext;
using EventBookingAPI.DTOs;
using EventBookingAPI.Helpers;
using EventBookingAPI.Interfaces;
using EventBookingAPI.Models;
using EventBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsersRepository _userRepository;
        private readonly JwtAuthenticationService _jwtAuthenticationService;

        private readonly IConfiguration _configuration;
        public AuthController(IUsersRepository userRepository, IConfiguration configuration, JwtAuthenticationService jwtAuthenticationService)
        {
            _userRepository = userRepository;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtAuthenticationService = jwtAuthenticationService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid input"));

            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(dto.Email!);
                if (existingUser != null)
                    return BadRequest(ApiResponse<string>.Fail("Email already exists"));

                int roleId = 2; // default User
                if (dto.RoleId.HasValue)
                {
                    // Only allow if user is authenticated and Admin
                    if (!User.Identity?.IsAuthenticated ?? true)
                        return Unauthorized(ApiResponse<string>.Fail("Only Admin can assign roles"));

                    var currentUserRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                    if (currentUserRole != "Admin")
                        return Unauthorized(ApiResponse<string>.Fail("Only Admin can assign roles"));

                    roleId = dto.RoleId.Value;
                }

                var user = new User
                {
                    Username = dto.UserName!,
                    Email = dto.Email!,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password!),
                    RoleId = roleId,
                    IsVerified = false,
                    Otp = new Random().Next(100000, 999999).ToString(),
                    OtpExpiry = DateTime.UtcNow.AddMinutes(10)
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // Send OTP asynchronously

                string subject = "Verify your Email";
                string body = $"Your OTP is: <b>{user.Otp}</b>. It expires in 10 minutes.";
                var result = await EmailHelper.SendEmailAsync(user.Email!, subject, body);

                if (!result.Success)
                {
                    await _userRepository.Delete(user);
                    await _userRepository.SaveChangesAsync();
                    return BadRequest(new { Message = result.Message });
                }
                    return StatusCode(201, ApiResponse<string>.Ok("Check your email for OTP", "Registration successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail($"Internal server error: {ex.Message}"));
            }
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyEmailDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email!);
            if (user == null) return NotFound(ApiResponse<string>.Fail("User not found"));

            if (user.Otp != dto.Otp || user.OtpExpiry < DateTime.UtcNow)
                return BadRequest(ApiResponse<string>.Fail("Invalid or expired OTP"));

            user.IsVerified = true;
            user.Otp = null;
            user.OtpExpiry = null;
            await _userRepository.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Email verified successfully"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _jwtAuthenticationService.Authenticate(dto);

            return result is not null ? result : Unauthorized();
        }


        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // Access claims here
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new { UserId = userId, Email = email, Role = role });
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace EventBookingAPI.DTOs
{
    // Registration DTO
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }

        // Optional: only Admin can assign roles during creation
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Role Id")]
        public int? RoleId { get; set; }
    }

    // Email Verification DTO
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits")]
        public string? Otp { get; set; }
    }

    // Login DTO
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }
    }

    // Login Response DTO (JWT + RefreshToken)
    public class LoginResponseDto
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
    }
}

namespace EventBookingAPI.DTOs
{
    public class RegisterDto { public string? UserName; public string? Email; public string? Password; }
    public class VerifyEmailDto { public string? Email; public string? Otp; }
    public class LoginDto { public string? Email; public string? Password; }
}

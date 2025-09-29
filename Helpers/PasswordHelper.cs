namespace EventBookingAPI.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var res = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return res;
        }
    }
}

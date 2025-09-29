namespace EventBookingAPI.Models
{
    public class RefreshTokenModel
    {
        public string? Token { get; set; }
        public int? UserId { get; set; }
        public DateTime? Expiry { get; set; }
        
    }
}

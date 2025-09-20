using System;
using System.Collections.Generic;

namespace EventBookingAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsVerified { get; set; }

    public string? Otp { get; set; }

    public DateTime? OtpExpiry { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

using System;
using System.Collections.Generic;

namespace EventBookingAPI.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public DateTime BookingTime { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

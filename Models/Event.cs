using System;
using System.Collections.Generic;

namespace EventBookingAPI.Models;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Capacity { get; set; }

    public int BookedCount { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

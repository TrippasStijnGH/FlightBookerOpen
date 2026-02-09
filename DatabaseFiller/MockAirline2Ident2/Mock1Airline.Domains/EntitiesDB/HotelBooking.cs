using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class HotelBooking
{
    public string HotelBookingId { get; set; } = null!;

    public string? City { get; set; }

    public DateTime? Checkin { get; set; }

    public DateTime? Checkout { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

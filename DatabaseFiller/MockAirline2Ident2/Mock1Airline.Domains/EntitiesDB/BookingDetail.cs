using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class BookingDetail
{
    public int BookingDetailId { get; set; }

    public int BookingId { get; set; }

    public int FlightId { get; set; }

    public int? MealId { get; set; }

    public int? SeatNumber { get; set; }

    public decimal Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Flight Flight { get; set; } = null!;

    public virtual Meal? Meal { get; set; }
}

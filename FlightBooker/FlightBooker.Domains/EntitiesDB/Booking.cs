using System;
using System.Collections.Generic;

namespace FlightBooker.Domains.EntitiesDB;

public partial class Booking
{
    public int BookingId { get; set; }

    public string UserId { get; set; } = null!;

    public string Class { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime BookingDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? HotelBooking { get; set; }

    public bool? EmailSent { get; set; }

    public int? DepartureCity { get; set; }

    public int? ArrivalCity { get; set; }

    public DateTime? DepartureDate { get; set; }

    public DateTime? ArrivalDate { get; set; }

    public virtual City? ArrivalCityNavigation { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual City? DepartureCityNavigation { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}

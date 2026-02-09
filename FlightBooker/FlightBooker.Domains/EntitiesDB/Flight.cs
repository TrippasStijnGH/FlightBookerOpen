using System;
using System.Collections.Generic;

namespace FlightBooker.Domains.EntitiesDB;

public partial class Flight
{
    public int FlightId { get; set; }

    public int DepartCity { get; set; }

    public int ArriveCity { get; set; }

    public DateTime? DateTimeDepart { get; set; }

    public DateTime? DateTimeArrive { get; set; }

    public decimal BasePrice { get; set; }

    public int RouteId { get; set; }

    public int? SequenceNumber { get; set; }

    public int? JourneyId { get; set; }

    public virtual City ArriveCityNavigation { get; set; } = null!;

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual City DepartCityNavigation { get; set; } = null!;

    public virtual ICollection<FlightClass> FlightClasses { get; set; } = new List<FlightClass>();

    public virtual RouteItem Route { get; set; } = null!;
}

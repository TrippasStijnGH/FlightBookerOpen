using System;
using System.Collections.Generic;

namespace FlightBooker.Domains.EntitiesDB;

public partial class RouteItem
{
    public int RouteId { get; set; }

    public int DepartCity { get; set; }

    public int ArrivalCity { get; set; }

    public bool Direct { get; set; }

    public virtual City ArrivalCityNavigation { get; set; } = null!;

    public virtual City DepartCityNavigation { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}

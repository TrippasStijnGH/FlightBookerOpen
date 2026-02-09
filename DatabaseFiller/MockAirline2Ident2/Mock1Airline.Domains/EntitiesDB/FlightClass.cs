using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class FlightClass
{
    public int FlightId { get; set; }

    public string ClassType { get; set; } = null!;

    public int MaxBookings { get; set; }

    public virtual Flight Flight { get; set; } = null!;
}

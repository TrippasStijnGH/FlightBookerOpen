using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class FlightRoutesFill
{
    public int FlightRouteIds { get; set; }

    public int ParentRouteIds { get; set; }

    public int OwnRouteIds { get; set; }

    public int SequenceNumber { get; set; }

    public int? FlightTime { get; set; }

    public decimal Price { get; set; }

    public int DepartCityS { get; set; }

    public int ArrivalCityS { get; set; }
}

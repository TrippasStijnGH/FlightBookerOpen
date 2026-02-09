using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class FlightRoute
{
    public int FlightRouteId { get; set; }

    public int RouteId { get; set; }

    public int FlightId { get; set; }

    public int SequenceNumber { get; set; }

    public int? JourneyId { get; set; }
}

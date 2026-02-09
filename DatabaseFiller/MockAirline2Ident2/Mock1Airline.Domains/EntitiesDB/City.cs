using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class City
{
    public string CityName { get; set; } = null!;

    public int? Utcoffset { get; set; }

    public string? FullName { get; set; }

    public int CityId { get; set; }

    public virtual ICollection<Booking> BookingArrivalCityNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingDepartureCityNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Flight> FlightArriveCityNavigations { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartCityNavigations { get; set; } = new List<Flight>();

    public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();

    public virtual ICollection<RouteItem> RouteItemArrivalCityNavigations { get; set; } = new List<RouteItem>();

    public virtual ICollection<RouteItem> RouteItemDepartCityNavigations { get; set; } = new List<RouteItem>();
}

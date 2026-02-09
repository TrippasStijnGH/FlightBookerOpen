using System;
using System.Collections.Generic;

namespace FlightBooker.Domains.EntitiesDB;

public partial class Meal
{
    public int MealId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? AreaCode { get; set; }

    public virtual City? AreaCodeNavigation { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
}

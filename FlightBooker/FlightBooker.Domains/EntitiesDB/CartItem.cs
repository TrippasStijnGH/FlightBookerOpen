using System;
using System.Collections.Generic;

namespace FlightBooker.Domains.EntitiesDB;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ShoppingCartId { get; set; }

    public int BookingId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}

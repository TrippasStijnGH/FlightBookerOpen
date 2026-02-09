using System;
using System.Collections.Generic;

namespace Mock1Airline.Domains.EntitiesDB;

public partial class ShoppingCart
{
    public int ShoppingCartId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual AspNetUser User { get; set; } = null!;
}

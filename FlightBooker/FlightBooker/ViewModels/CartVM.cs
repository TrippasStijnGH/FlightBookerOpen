namespace FlightBooker.ViewModels
{
    public class CartVM
    {
        public List<CartItemVM>? Carts { get; set; }

        public decimal ComputeTotalValue() =>
        Carts?.Sum(e => e.Price) ?? 0;

    }

    public class CartItemVM
    {
        public int cartId { get; set; }
        public string TravelClass { get; set; }
        public string? Class { get; set; }
        public List<FlightVM> Flights { get; set; }

        public decimal Price { get; set; } = 0;

        public int ?bookingID { get; set; } = null;
        public System.DateTime DateCreated { get; set; }

    }
}



namespace FlightBooker.ViewModels
{
    public class ShoppingCartContentVM
    {
        public CartVM Cart { get; set; }

        public string TravelClass { get; set; }

        public string FlightIds { get; set; }

        public string BookingId { get; set; }

        public int? SelectedCartItemId { get; set; }

        public DateTime? FlightDateDepart { get; set; }

        public DateTime? FlightDateDepartArrive { get; set; }
    }
}

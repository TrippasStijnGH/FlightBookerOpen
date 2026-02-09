namespace FlightBooker.ViewModels
{
    public class MyBookingsVM
    {
        public IEnumerable<BookingVM> Bookings { get; set; }

        public int BookingId { get; set; }
    }
}

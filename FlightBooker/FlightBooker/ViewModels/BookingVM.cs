using FlightBooker.Domains.EntitiesDB;

namespace FlightBooker.ViewModels
{
    public class BookingVM
    {

        public int BookingId { get; set; }
        public string Class { get; set; } = null!;

        public string Status { get; set; } = null!;

        public decimal Price { get; set; }

        public List<FlightVM>? Flights { get; set; }

        public string DepartureCity { get; set; }

        public string ArrivalCity {  get; set; }    

        public DateTime BookingDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? HotelBooking { get; set; }

        public bool? EmailSent { get; set; }

        public DateTime DepartureTimeFirst { get; set; }

        public DateTime DepartureTimeLast { get; set; }

        public DateTime ArrivalTimeFirst { get; set; }

        public DateTime ArrivalTimeLast { get; set; }




    }

    


}

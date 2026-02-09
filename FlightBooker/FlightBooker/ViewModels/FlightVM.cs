using FlightBooker.Domains.EntitiesDB;

namespace FlightBooker.ViewModels
{
    public class FlightVM
    {
        public int FlightID { get; set; }   
        public int DepartCity { get; set; }

        public string DepartCityName { get; set; }

        public int ArriveCity { get; set; }

        public string ArriveCityName { get; set; }

        public DateTime DateTimeDepart { get; set; }

        public DateTime LocalDateTimeDepart { get; set; }

        public DateTime DateTimeArrive { get; set; }
        public DateTime LocalDateTimeArrive { get; set; }

        public decimal BasePrice { get; set; }

        public decimal RealPrice { get; set; }

        public string? Meal { get; set; }

        public int? SeatNumber { get; set; }


    }
}

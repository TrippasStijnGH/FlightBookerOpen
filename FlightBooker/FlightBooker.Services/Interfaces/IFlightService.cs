using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Services.Interfaces
{
    public interface IFlightService : IService<Flight>
    {
        // Add specialized methods here
        Task<IEnumerable<IEnumerable<Flight>>> SearchFlightsAsync(int departCity, int arrivalCity,
                                                  DateTime startDate, DateTime endDate, string Class);

        Task<IEnumerable<Flight>?> GetFlightsByRouteAsync(int departCity, int arrivalCity);

        Task<IEnumerable<Flight>?> GetFlightsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /*Task<bool> IsFlightAvailableAsync(int flightId, string classType, int numberOfSeats);*/

        Task<List<Flight>> GetFlightsForBookingAsync(int bookingId);

        /*Task<decimal?> CalculateRealPrice(Flight flight);*/
    }
}

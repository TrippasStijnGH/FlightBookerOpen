using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Repositories.Interfaces
{
    public interface IFlightDAO : IDAO<Flight>
    {
        Task<IEnumerable<IEnumerable<Flight>>> SearchFlightsAsync(int departCity, int arrivalCity,
                                                 DateTime startDate, DateTime endDate, string Class);

        Task<IEnumerable<Flight>?> GetFlightsByRouteAsync(int departCity, int arrivalCity);

        Task<IEnumerable<Flight>?> GetFlightsByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> IsFlightAvailableAsync(int flightId, string classType, int numberOfSeats);

        Task<List<Flight>> GetFlightsForBookingAsync(int bookingId);

    }
}

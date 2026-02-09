using FlightBooker.Domains.EntitiesDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Repositories.Interfaces
{
    public interface IBookingDetailDAO : IDAO<BookingDetail>
    {
        Task<IEnumerable<int?>> BookingsInFlightClassAsync(int flight, string travelClass);

        Task<BookingDetail> FindByBookingAndFlight(int bookingId, int flightId);


    }
}

using FlightBooker.Domains.EntitiesDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Services.Interfaces
{
    public interface IBookingService : IService<Booking>

    {
        Task<IEnumerable<Booking>> FindByUserIdAsync(string userId);

        Task<int> CreateBookingAsync(int[] flightIds, string classType, string userId, bool cart = false);

        decimal CalculateRealPrice(Flight flight, string Class);
        Task<bool> IsBookingAvailable(int bookingId);

        Task<IEnumerable<int?>> BookingsInFlightClassAsync(int flightId, string travelClass);

        Task UpdateFlightMealAsync(int bookingId, int flightId, int mealId);

        Task CancelBookingAsync(int bookingId);

        Task ConfirmBookingAsync(int bookingId);

        Task<int> PopularDestinationUser(String userId);


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooker.Repositories
{
    public class FlightDAO : IFlightDAO
    {
        private readonly FlightBookingDbContext dbContext;

        public FlightDAO(FlightBookingDbContext context)
        {
            dbContext = context;
        }
          
        public async Task<Flight?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.Flights
                    .Where(f => f.FlightId == id)
                    .Include(f => f.DepartCityNavigation)
                    .Include(f => f.ArriveCityNavigation)
                    .Include(f => f.FlightClasses)
                    .Include(f => f.Route)
                        
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task<IEnumerable<Flight>?> GetAllAsync()
        {
            try
            {
                return await dbContext.Flights
                    .Include(f => f.DepartCityNavigation)
                    .Include(f => f.ArriveCityNavigation)
                    .Include(f => f.FlightClasses)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task AddAsync(Flight entity)
        {
            dbContext.Entry(entity).State = EntityState.Added;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task UpdateAsync(Flight entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task DeleteAsync(Flight entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }


        public async Task<IEnumerable<IEnumerable<Flight>?>> SearchFlightsAsync(int departCity, int arriveCity, DateTime startDate, DateTime endDate, string Class)
        {
            List<List<Flight>> answer = new List<List<Flight>>();

            var route = await dbContext.RouteItems
            .Where(r => r.DepartCity == departCity && r.ArrivalCity == arriveCity)
            .FirstOrDefaultAsync();

            if (route.Direct == true)
            {

                var directflights = await dbContext.Flights
                .Where(f => f.DateTimeDepart >= startDate && f.DateTimeDepart <= endDate)
                .Where(f => f.DepartCity == departCity && f.ArriveCity == arriveCity)
                .Include(f => f.DepartCityNavigation)
                .Include(f => f.ArriveCityNavigation)
                .Include(f => f.FlightClasses)
                .Include(f => f.Route)
                .ToListAsync();

                var listflights = new List<List<Flight>>();
                foreach (var flight in directflights)
                {
                    var aflight = new List<Flight>();
                    aflight.Add(flight);
                    listflights.Add(aflight);
                }
                answer = listflights;
            }
            else
            {
                var flightsGrouped = await dbContext.Flights
                    .Where(f => f.RouteId == route.RouteId)
                    .Where(f => f.DateTimeDepart >= startDate && f.DateTimeDepart <= endDate)
                    .Include(f => f.DepartCityNavigation)
                    .Include(f => f.ArriveCityNavigation)
                    .Include(f => f.FlightClasses)
                    .Include(f => f.Route)
                    .GroupBy(fr => fr.JourneyId)
                    .Select(group => group.OrderBy(f => f.SequenceNumber)
                                         .ToList())
                    .ToListAsync();

                answer = flightsGrouped;
            }

            List<List<Flight>> answerchecked = new List<List<Flight>>();

            foreach (List<Flight> flights in answer)
            {
                bool groupAvailable = true;

                foreach (Flight f in flights)
                {
                    bool available = await IsFlightAvailableAsync(f.FlightId, Class, 1);
                    if (!available)
                    {
                        groupAvailable = false;
                        break; 
                    }
                }
                if (groupAvailable)
                {
                    answerchecked.Add(flights); 
                }
            }
            return answerchecked;

            
        }
     
        public async Task<IEnumerable<Flight>?> GetFlightsByRouteAsync(int departCity, int arriveCity)
        {
            try
            {
                return await dbContext.Flights
                    .Where(f => f.DepartCity == departCity && f.ArriveCity == arriveCity)
                    .Include(f => f.DepartCityNavigation)
                    .Include(f => f.ArriveCityNavigation)
                    .Include(f => f.FlightClasses)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task<IEnumerable<Flight>?> GetFlightsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await dbContext.Flights
                    .Where(f => f.DateTimeDepart >= startDate && f.DateTimeDepart <= endDate)
                    .Include(f => f.DepartCityNavigation)
                    .Include(f => f.ArriveCityNavigation)
                    .OrderBy(f => f.DateTimeDepart)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task<List<Flight>> GetFlightsForBookingAsync(int bookingId)
        {
            try
            {
                
                var booking = await dbContext.Bookings
                    .Include(b => b.BookingDetails)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                    return new List<Flight>();

                
                var flightIds = booking.BookingDetails.Select(bd => bd.FlightId).ToList();

               
                var flights = await dbContext.Flights
                    .Where(f => flightIds.Contains(f.FlightId))
                    .ToListAsync();

                return flights;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task<IEnumerable<Flight>?> FindAvailableFlightsAsync(int departCity, int arriveCity, DateTime departDate)
        {
            try
            {
                
                var startOfDay = departDate.Date;
                var endOfDay = departDate.Date.AddDays(1).AddTicks(-1);

                return await dbContext.Flights
                    .Where(f => f.DepartCity == departCity &&
                                f.ArriveCity == arriveCity &&
                                f.DateTimeDepart >= startOfDay &&
                                f.DateTimeDepart <= endOfDay)
                    .Include(f => f.DepartCityNavigation)
                    .Include(f => f.ArriveCityNavigation)
                    .Include(f => f.FlightClasses)
                    .OrderBy(f => f.DateTimeDepart)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightDAO", ex);
            }
        }

        public async Task<bool> IsFlightAvailableAsync(int flightId, string classType, int numberOfSeats)
        {
            try
            {
                
                var flight = await dbContext.Flights
                    .Include(f => f.FlightClasses)
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);

                if (flight == null)
                    return false;

                
                var flightClass = flight.FlightClasses
                    .FirstOrDefault(fc => fc.ClassType.Equals(classType, StringComparison.OrdinalIgnoreCase));

                if (flightClass == null)
                    return false;

               

                var bookedSeats = await dbContext.BookingDetails
                    .Where(bd => bd.FlightId == flightId &&
                    bd.Booking.Class.ToLower() == classType.ToLower())
                    .CountAsync();


                int maxCapacity = flightClass.MaxBookings;

                
                return (maxCapacity - bookedSeats) >= numberOfSeats;
            }
            catch (Exception ex)
            {

                throw new Exception("Error in FlightDAO", ex);
            }




        }
    }
}
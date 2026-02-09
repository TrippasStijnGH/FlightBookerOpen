using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightBooker.Repositories.Interfaces;
using global::FlightBooker.Domains.DataDB;
using global::FlightBooker.Domains.EntitiesDB;
using Microsoft.EntityFrameworkCore;

namespace FlightBooker.Repositories
{
    public class BookingDAO : IBookingDAO
    {
        private readonly FlightBookingDbContext dbContext;
        private readonly IFlightDAO _flightDAO;

        public BookingDAO(FlightBookingDbContext context, IFlightDAO flightDAO)
        {
            dbContext = context;
        } 

        public async Task<Booking?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.Bookings
                    .Where(b => b.BookingId == id)
                    .Include(b => b.User)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Flight)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Meal)
                    .Include(b => b.ArrivalCityNavigation)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDAO", ex);
            }
        }

        public async Task<IEnumerable<Booking>?> GetAllAsync()
        {
            try
            {
                return await dbContext.Bookings
                    .Include(b => b.User)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Flight)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Meal)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDAO", ex);
            }
        }

        public async Task AddAsync(Booking entity)
        {
            
            dbContext.Bookings.Add(entity);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        public async Task UpdateAsync(Booking entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task DeleteAsync(Booking entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        
        public async Task<IEnumerable<Booking>?> FindByUserIdAsync(string userId)
        {
            try
            {
                return await dbContext.Bookings
                    .Where(b => b.UserId == userId)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Flight)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Meal)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDAO", ex);
            }
        }

        public async Task<IEnumerable<Booking>?> FindByStatusAsync(string status)
        {
            try
            {
                return await dbContext.Bookings
                    .Where(b => b.Status == status)
                    .Include(b => b.User)
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Flight)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDAO", ex);
            }
        }


        public async Task<int> CreateBookingAsync(IEnumerable<int> flightIds, string classType, string userId = null)
        {
           
            
            var booking = new Booking
            {
                UserId = userId,
                BookingDate = DateTime.UtcNow,
                Class = classType,

                Price = 0 
            };

            
            decimal totalPrice = 0;
            foreach (var flightId in flightIds)
            {
                var flight = await dbContext.Flights
                    .Include(f => f.FlightClasses)
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);


                
                var detail = new BookingDetail
                {
                    Flight = flight,
                    Booking = booking,
                    Price = flight.BasePrice 
                };

                booking.BookingDetails.Add(detail);
                totalPrice += detail.Price;
            }

            booking.Price = totalPrice;

            
            dbContext.Bookings.Add(booking);
            await dbContext.SaveChangesAsync();

            return booking.BookingId;
        }


        public async Task<int?> PopularDestinationUser(String userId)
        {
            var userBookings = await dbContext.Bookings
         .Where(b => b.UserId == userId)
         .Include(f => f.BookingDetails)
            .ThenInclude(bd => bd.Flight)
         .ToListAsync();

            if (!userBookings.Any())
                return null; 

            
            var destinationCounts = new Dictionary<int, int>();

            
            foreach (var booking in userBookings)
            {
                
                var details = booking.BookingDetails;
                var flights = booking.BookingDetails.Select(booking => booking.Flight).ToList();    
                if (flights.Any())
                {
                    var flightsList = flights.ToList().OrderBy(f => f.DateTimeDepart).ToList();
                    var arrivalCity = flightsList.Last().ArriveCity;

                    
                    if (destinationCounts.ContainsKey(arrivalCity))
                        destinationCounts[arrivalCity]++;
                    else
                        destinationCounts[arrivalCity] = 1;
                }
            }

            
            if (!destinationCounts.Any())
                return null;

            return destinationCounts.OrderByDescending(x => x.Value).First().Key;

            
        }
    }
}
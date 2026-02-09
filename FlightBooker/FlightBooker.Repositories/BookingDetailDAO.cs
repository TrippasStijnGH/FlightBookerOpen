using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Repositories
{
    public class BookingDetailDAO : IBookingDetailDAO

    {
        private readonly FlightBookingDbContext dbContext; // database context  

        public BookingDetailDAO(FlightBookingDbContext context)  // DI
        {
            dbContext = context;
        }

        public async Task<BookingDetail?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.BookingDetails
                    .Where(r => r.BookingDetailId == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDetailDAO", ex);
            }
        }

        public async Task AddAsync(BookingDetail entity)
        {
            dbContext.Entry(entity).State = EntityState.Added;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDetailDAO", ex);
            }

            }

            public async Task DeleteAsync(BookingDetail entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDetailDAO", ex);
            }
        }

        public async Task<IEnumerable<BookingDetail>?> GetAllAsync()
        {
            try
            {
                return await dbContext.BookingDetails

                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDetailDAO", ex);
            }
        }

        public async Task UpdateAsync(BookingDetail entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BookingDetailDAO", ex);
            }
        }

        public async Task<IEnumerable<int?>> BookingsInFlightClassAsync(int flight, string travelClass)
        {
            return await dbContext.BookingDetails
                .Join(dbContext.Bookings,
                      bd => bd.BookingId,
                      b => b.BookingId,
                      (bd, b) => new { bd, b})
                .Where(x => x.bd.FlightId == flight &&
                       x.b.Class == travelClass &&
                       x.b.Status == "Confirmed")
                .Select(x => x.bd.SeatNumber)
                .ToListAsync();
        }

        public async Task<BookingDetail> FindByBookingAndFlight(int bookingId, int flightId)
        {
            var bookingDetail = await dbContext.BookingDetails
             .FirstOrDefaultAsync(bd => bd.BookingId == bookingId && bd.FlightId == flightId);

            return bookingDetail;
           
        }

        

        

    }
}

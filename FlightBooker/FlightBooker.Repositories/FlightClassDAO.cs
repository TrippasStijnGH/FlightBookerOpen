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
    public class FlightClassDAO : IFlightClassDAO
    {
        private readonly FlightBookingDbContext dbContext;

        public FlightClassDAO(FlightBookingDbContext context)
        {
            dbContext = context;
        }


        public async Task<FlightClass?> FindByIdAsync(int id)
        {
            return await Task.FromResult<FlightClass?>(null);
        }

        public async Task AddAsync(FlightClass entity)
        {
            dbContext.Entry(entity).State = EntityState.Added;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightClassDAO", ex);
            }
        }

        public async Task DeleteAsync(FlightClass entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightClassDAO", ex);
            }
        }

        public async Task<IEnumerable<FlightClass>?> GetAllAsync()
        {
            try
            {
                return await dbContext.FlightClasses
                    .Include(fc => fc.Flight)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightClassDAO", ex);
            }
        }

        public async Task UpdateAsync(FlightClass entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FlightClassDAO", ex);
            }
        }
        
        public async Task<IEnumerable<FlightClass>?> GetFCsByFlightAsync(int flightId)
        {
            return await dbContext.FlightClasses
                .Where(fc => fc.FlightId == flightId)
                .ToListAsync();


        }


    }
}

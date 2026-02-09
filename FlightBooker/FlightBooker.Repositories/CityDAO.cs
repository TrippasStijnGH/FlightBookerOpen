using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using Microsoft.EntityFrameworkCore;

namespace FlightBooker.Repositories
{
    public class CityDAO : IDAO<City>
    {
        private readonly FlightBookingDbContext dbContext;

        public CityDAO(FlightBookingDbContext context)
            
            
        {
            dbContext = context;
        }

        public async Task<City?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.Cities
                    .Where(c => c.CityId == id)
                    
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cityDAO", ex);
            }
        }

        public async Task<City?> FindByCityNameAsync(string cityName)
        {
            try
            {
               
                return await dbContext.Cities
                    .FirstOrDefaultAsync(c => c.CityName == cityName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cityDAO", ex);
            }
        }

        public async Task<IEnumerable<City>?> GetAllAsync()
        {
            try
            {
                
                return await dbContext.Cities
                    .OrderBy(c => c.CityName)
                    .ToListAsync();
               
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cityDAO", ex);
            }
        }

        public async Task AddAsync(City entity)
        {
            try
            {
                await dbContext.Cities.AddAsync(entity);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cityDAO", ex);
            }
        }

        public async Task UpdateAsync(City entity)
        {
            try
            {
                dbContext.Entry(entity).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cityDAO", ex);
            }
        }

        public async Task DeleteAsync(City entity)
        {
            try
            {
                dbContext.Cities.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cityDAO", ex);
            }
        }
    }
}
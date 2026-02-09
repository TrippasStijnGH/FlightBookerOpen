using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using Microsoft.EntityFrameworkCore;

namespace FlightBooker.Repositories
{
    public class RouteItemDAO : IDAO<RouteItem>
    {
        private readonly FlightBookingDbContext dbContext; 

        public RouteItemDAO(FlightBookingDbContext context)  
        {
            dbContext = context;
        }

        public async Task<RouteItem?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.RouteItems
                    .Where(r => r.RouteId == id)
                    .Include(r => r.DepartCityNavigation)
                    .Include(r => r.ArrivalCityNavigation)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task AddAsync(RouteItem entity)
        {
            dbContext.Entry(entity).State = EntityState.Added;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task DeleteAsync(RouteItem entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task<IEnumerable<RouteItem>?> GetAllAsync()
        {
            try
            {
                return await dbContext.RouteItems
                    .Include(r => r.DepartCityNavigation)
                    .Include(r => r.ArrivalCityNavigation)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task UpdateAsync(RouteItem entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        
        //niet gebruikt
        public async Task<RouteItem?> FindRouteByDepartureAndArrivalAsync(int departCity, int arrivalCity)
        {
            try
            {
                return await dbContext.RouteItems
                    .Where(r => r.DepartCity == departCity && r.ArrivalCity == arrivalCity)
                    .Include(r => r.DepartCityNavigation)
                    .Include(r => r.ArrivalCityNavigation)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task<IEnumerable<RouteItem>?> FindRoutesByDepartureCityAsync(int departCity)
        {
            try
            {
                return await dbContext.RouteItems
                    .Where(r => r.DepartCity == departCity)
                    .Include(r => r.DepartCityNavigation)
                    .Include(r => r.ArrivalCityNavigation)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task<IEnumerable<RouteItem>?> FindRoutesByArrivalCityAsync(int arrivalCity)
        {
            try
            {
                return await dbContext.RouteItems
                    .Where(r => r.ArrivalCity == arrivalCity)
                    .Include(r => r.DepartCityNavigation)
                    .Include(r => r.ArrivalCityNavigation)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }

        public async Task<IEnumerable<RouteItem>?> FindDirectRoutesAsync()
        {
            try
            {
                return await dbContext.RouteItems
                    .Where(r => r.Direct == true)
                    .Include(r => r.DepartCityNavigation)
                    .Include(r => r.ArrivalCityNavigation)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in RouteDAO", ex);
            }
        }
    }
}
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
    public class MealDAO : IDAO<Meal>
    {
        private readonly FlightBookingDbContext dbContext;

        public MealDAO(FlightBookingDbContext context)


        {
            dbContext = context;
        }

        public async Task<Meal?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.Meals
                    .Where(c => c.MealId == id)

                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MealDAO", ex);
            }
        }

        public async Task<Meal?> FindByMealNameAsync(string MealName)
        {
            try
            {

                return await dbContext.Meals
                    .FirstOrDefaultAsync(c => c.Name == MealName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MealDAO", ex);
            }
        }

        public async Task<IEnumerable<Meal>?> GetAllAsync()
        {
            try
            {

                return await dbContext.Meals
                    .OrderBy(c => c.Name)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error in MealDAO", ex);
            }
        }

        public async Task AddAsync(Meal entity)
        {
            try
            {
                await dbContext.Meals.AddAsync(entity);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MealDAO", ex);
            }
        }

        public async Task UpdateAsync(Meal entity)
        {
            try
            {
                dbContext.Entry(entity).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MealDAO", ex);
            }
        }

        public async Task DeleteAsync(Meal entity)
        {
            try
            {
                dbContext.Meals.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MealDAO", ex);
            }
        }
    }
}

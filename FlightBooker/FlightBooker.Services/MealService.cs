using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Repositories.Interfaces;
using FlightBooker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace FlightBooker.Services
{
    public class MealService : IService<Meal>
    {
        private readonly IDAO<Meal> _MealDAO;
        

        public MealService(IDAO<Meal> MealDAO)
        {
            _MealDAO = MealDAO;
            
        }

        public async Task<Meal?> FindByIdAsync(int id)
        {
           return await _MealDAO.FindByIdAsync(id);
        }


        public async Task<IEnumerable<Meal>?> GetAllAsync()
        {
            return await _MealDAO.GetAllAsync();
        }

        public async Task AddAsync(Meal entity)
        {
            await _MealDAO.AddAsync(entity);
        }

        public async Task UpdateAsync(Meal entity)
        {
            await _MealDAO.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Meal entity)
        {
            await _MealDAO.DeleteAsync(entity);
        }
    }
}

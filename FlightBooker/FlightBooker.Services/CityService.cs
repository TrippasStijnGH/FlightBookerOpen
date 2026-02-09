using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBooker.Services
{
    public class CityService : IService<City>
    {
        private readonly IDAO<City> _cityDAO;
        private readonly CityDAO _extendedCityDAO;

        public CityService(IDAO<City> cityDAO)
        {
            _cityDAO = cityDAO;
 
        }

        public async Task<City?> FindByIdAsync(int id)
        {
            return await _cityDAO.FindByIdAsync(id);
        }



        public async Task<IEnumerable<City>?> GetAllAsync()
        {
            return await _cityDAO.GetAllAsync();
        }

        public async Task AddAsync(City entity)
        {
            await _cityDAO.AddAsync(entity);
        }

        public async Task UpdateAsync(City entity)
        {
            await _cityDAO.UpdateAsync(entity);
        }

        public async Task DeleteAsync(City entity)
        {
            await _cityDAO.DeleteAsync(entity);
        }
    }
}
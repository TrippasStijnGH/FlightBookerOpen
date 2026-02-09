using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Repositories.Interfaces;
using FlightBooker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FlightBooker.Services
{

    public class FlightClassService : IFlightClassService
    {
        private readonly IFlightClassDAO _flightClassDAO;

        public FlightClassService(IFlightClassDAO flightClassDAO)
        {
            _flightClassDAO = flightClassDAO;

        }
        public async Task<IEnumerable<FlightClass?>> GetFCsByFlightAsync(int FlightId)
        {
            return await _flightClassDAO.GetFCsByFlightAsync(FlightId);
        }

        public async Task<FlightClass?> FindByIdAsync(int id)
        {
            return await _flightClassDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<FlightClass>?> GetAllAsync()
        {
            return await _flightClassDAO.GetAllAsync();
        }

        public async Task AddAsync(FlightClass entity)
        {
            await _flightClassDAO.AddAsync(entity);
        }

        public async Task UpdateAsync(FlightClass entity)
        {
            await _flightClassDAO.UpdateAsync(entity);
        }

        public async Task DeleteAsync(FlightClass entity)
        {
            await _flightClassDAO.DeleteAsync(entity);
        }
    }


}


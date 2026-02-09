using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories;
using FlightBooker.Repositories.Interfaces;

using FlightBooker.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightDAO _flightDAO;
        
        
        

        public FlightService(IFlightDAO flightDAO) 
        {
            _flightDAO = flightDAO;
            
            
        }

        

            public async Task<Flight?> FindByIdAsync(int id)
        {
            return await _flightDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Flight>?> GetAllAsync()
        {
            return await _flightDAO.GetAllAsync();
        }

        public async Task AddAsync(Flight entity)
        {
            await _flightDAO.AddAsync(entity);
        }

        public async Task UpdateAsync(Flight entity)
        {
            await _flightDAO.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Flight entity)
        {
            await _flightDAO.DeleteAsync(entity);
        }

        
        public async Task<IEnumerable<IEnumerable<Flight>?>> SearchFlightsAsync(int departCity, int arriveCity, DateTime departDate, DateTime ArriveDate, string Class)
        {
            return await _flightDAO.SearchFlightsAsync(departCity, arriveCity, departDate, ArriveDate,Class);
        }

        public async Task<IEnumerable<Flight>?> GetFlightsByRouteAsync(int departCity, int arriveCity)
        {
            return await _flightDAO.GetFlightsByRouteAsync(departCity, arriveCity);
        }

        public async Task<IEnumerable<Flight>?> GetFlightsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _flightDAO.GetFlightsByDateRangeAsync(startDate, endDate);
        }

        public async Task<List<Flight>> GetFlightsForBookingAsync(int bookingId)
        {
            return await _flightDAO.GetFlightsForBookingAsync(bookingId);
        }
      
    }
}

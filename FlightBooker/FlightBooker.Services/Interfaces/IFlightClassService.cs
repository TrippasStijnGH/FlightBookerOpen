using FlightBooker.Domains.EntitiesDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Services.Interfaces
{
    public interface IFlightClassService : IService<FlightClass>
    {
        Task<IEnumerable<FlightClass>?> GetFCsByFlightAsync(int FlightId);

    }


   
}

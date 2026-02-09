using FlightBooker.Domains.EntitiesDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Repositories.Interfaces
{
    public interface IFlightClassDAO : IDAO<FlightClass>
    {
        Task<IEnumerable<FlightClass>?> GetFCsByFlightAsync(int FlightId);
    }
}

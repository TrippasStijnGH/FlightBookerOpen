using FlightBooker.Domains.EntitiesDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Repositories.Interfaces
{
    public interface IBookingDAO : IDAO<Booking>
    
    {

        Task<IEnumerable<Booking>> FindByUserIdAsync(string userId);

        Task<int> CreateBookingAsync(IEnumerable<int> flightIds, string classType, string userId = null);

        Task<int?> PopularDestinationUser(String userId);


    }
}

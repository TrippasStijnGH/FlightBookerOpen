using FlightBooker.Domains.EntitiesDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Services.Interfaces
{

    public interface IShoppingCartService : IService<ShoppingCart>


    {

        Task<ShoppingCart?> GetOrCreateCartForUserAsync(string userId);
        Task AddToCartForUser(int bookingId, string userId);

        Task RemoveItemFromCartAsync(int cartItemId);
    }
}

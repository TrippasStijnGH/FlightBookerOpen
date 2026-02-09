using FlightBooker.Domains.EntitiesDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightBooker.Repositories.Interfaces
{
    
    public interface IShoppingCartDAO : IDAO<ShoppingCart>
    {
        Task<ShoppingCart?> FindByUserIdAsync(string userId);
        Task AddCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(int cartItemId);



    }
}

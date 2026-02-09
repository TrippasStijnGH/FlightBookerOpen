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
    public class ShoppingCartService : IShoppingCartService
    {

        private readonly IShoppingCartDAO _shoppingCartDAO;
        

        public ShoppingCartService(IShoppingCartDAO shoppingCartDAO)
        {
            _shoppingCartDAO = shoppingCartDAO;
            
        }

        public async Task<ShoppingCart?> FindByIdAsync(int id)
        {
            return await _shoppingCartDAO.FindByIdAsync(id);
        }

        public async Task<IEnumerable<ShoppingCart>?> GetAllAsync()
        {
            return await _shoppingCartDAO.GetAllAsync();
        }

        public async Task AddAsync(ShoppingCart entity)
        {
            await _shoppingCartDAO.AddAsync(entity);
        }

        public async Task UpdateAsync(ShoppingCart entity)
        {
            await _shoppingCartDAO.UpdateAsync(entity);
        }

        public async Task DeleteAsync(ShoppingCart entity)
        {
            await _shoppingCartDAO.DeleteAsync(entity);
        }




        public async Task AddToCartForUser(int bookingId, string userId)
        {
            bool newcart = false;
            var cart = await GetOrCreateCartForUserAsync(userId);
           

            var cartItem = new CartItem
            {
                BookingId = bookingId

            };

            cart.CartItems.Add(cartItem);

         
            await _shoppingCartDAO.UpdateAsync(cart);
            

            
        }


            public async Task<ShoppingCart?> GetOrCreateCartForUserAsync(string userId)
        {
            var cart = await _shoppingCartDAO.FindByUserIdAsync(userId);

            if (cart == null)
            {
                
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };

                await _shoppingCartDAO.AddAsync(cart);
            }

            return cart;
        }



        public async Task RemoveItemFromCartAsync(int cartItemId)
        {
            await _shoppingCartDAO.RemoveCartItemAsync(cartItemId);
        }

        public async Task<bool> ClearCartAsync(int cartId)
        {
            var cart = await _shoppingCartDAO.FindByIdAsync(cartId);
            if (cart == null)
                return false;

            
            foreach (var item in cart.CartItems.ToList())
            {
                await _shoppingCartDAO.RemoveCartItemAsync(item.CartItemId);
            }

            return true;
        }
    }
}

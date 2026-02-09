using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooker.Repositories
{
    public class ShoppingCartDAO : IShoppingCartDAO
    {
        private readonly FlightBookingDbContext dbContext;

        public ShoppingCartDAO(FlightBookingDbContext context)
        {
            dbContext = context;
        }

        public async Task<ShoppingCart?> FindByIdAsync(int id)
        {
            try
            {
                return await dbContext.ShoppingCarts
                    .Where(sc => sc.ShoppingCartId == id)
                    .Include(sc => sc.User)
                    .Include(sc => sc.CartItems)
                        .ThenInclude(ci => ci.Booking)
                            .ThenInclude(b => b.BookingDetails)
                                .ThenInclude(bd => bd.Flight)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ShoppingCartDAO.FindByIdAsync", ex);
            }
        }

        public async Task<IEnumerable<ShoppingCart>?> GetAllAsync()
        {
            try
            {
                return await dbContext.ShoppingCarts
                    .Include(sc => sc.User)
                    .Include(sc => sc.CartItems)
                        .ThenInclude(ci => ci.Booking)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ShoppingCartDAO.GetAllAsync", ex);
            }
        }

        public async Task AddAsync(ShoppingCart entity)
        {
            dbContext.Entry(entity).State = EntityState.Added;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task UpdateAsync(ShoppingCart entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task DeleteAsync(ShoppingCart entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        
        public async Task<ShoppingCart?> FindByUserIdAsync(string userId)
        {
            try
            {
                return await dbContext.ShoppingCarts
                    .Where(sc => sc.UserId == userId)
                    .Include(sc => sc.CartItems)
                        .ThenInclude(ci => ci.Booking)
                            .ThenInclude(b => b.BookingDetails)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ShoppingCartDAO.FindByUserIdAsync", ex);
            }
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            dbContext.CartItems.Add(cartItem);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await dbContext.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                dbContext.CartItems.Remove(cartItem);
                await dbContext.SaveChangesAsync();
            }
        }

      







    }
}
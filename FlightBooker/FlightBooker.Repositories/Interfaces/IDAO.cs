using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBooker.Repositories
{
    public interface IDAO<T> where T : class
    {
        Task<T?> FindByIdAsync(int id);
        Task<IEnumerable<T>?> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
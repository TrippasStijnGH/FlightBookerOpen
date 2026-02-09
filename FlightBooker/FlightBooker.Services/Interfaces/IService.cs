using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBooker.Services.Interfaces
{
    public interface IService<T>
    {
        Task<T?> FindByIdAsync(int id);
        Task<IEnumerable<T>?> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
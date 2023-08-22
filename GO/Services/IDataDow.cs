using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
   public interface IDataDow<T>
   {
        Task<bool> AddDOWAsync(T item);
        Task<bool> UpdateDOWAsync(T item);
        Task<bool> DeleteDOWAsync(int id);
        Task<T> GetDOWAsync(int id);     
        Task<IEnumerable<T>> GetDOWsAsync(bool forceRefresh = false);
    }
}

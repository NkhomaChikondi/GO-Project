using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IDataWeek<T>
    {
        Task<bool> AddWeekAsync(T item);
        Task<bool> UpdateWeekAsync(T item);
        Task<bool> DeleteWeekAsync(int id);
        Task<T> GetWeekAsync(int id);
        Task<IEnumerable<T>> GetWeeksAsync(int Id, bool forceRefresh = false);
       
    }
}

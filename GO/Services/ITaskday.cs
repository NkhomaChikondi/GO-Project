using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface ITaskday<T>
    {
        Task<bool> AddTaskdayAsync(T item);
        Task<bool> UpdateTaskdayAsync(T item);
        Task<bool> DeleteTaskdayAsync(int id);
        Task<T> GetTaskdayAsync(int id);
        Task<IEnumerable<T>> GetTaskdaysAsync( bool forceRefresh = false);

    }
}

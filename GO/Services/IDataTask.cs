using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IDataTask<T>
    {
        Task<bool> AddTaskAsync(T item);
        Task<bool> UpdateTaskAsync(T item);
        Task<bool> DeleteTaskAsync(int id);
        Task<T> GetTaskAsync(int id);
        Task<IEnumerable<T>> GetTasksAsync(int Id, bool forceRefresh = false);
        Task<IEnumerable<T>> GetTasksAsync(int Id, int weekid, bool forceRefresh = false);
    }
}

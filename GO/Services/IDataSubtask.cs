using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IDataSubtask<T>
    {

        Task<bool> AddSubTaskAsync(T item);
        Task<bool> UpdateSubTaskAsync(T item);
        Task<bool> DeleteSubTaskAsync(int id);
        Task<T> GetSubTaskAsync(int id);
        Task<IEnumerable<T>> GetSubTasksAsync(int Id, bool forceRefresh = false);
    }
}

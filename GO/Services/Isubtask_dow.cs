using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface Isubtask_dow<T>
    {
        Task<bool> AddSubtaskdowAsync(T item);
        Task<bool> UpdateSubtaskdowAsync(T item);
        Task<bool> DeleteSubtaskdowAsync(int id);
        Task<T> GetSubtaskdowAsync(int id);
        Task<IEnumerable<T>> GetSubtaskdowAsync(bool forceRefresh = false);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IDowGoal<T>
    {
        Task<bool> AddDowGoalAsync(T item);
        Task<bool> UpdateDowGoalAsync(T item);
        Task<bool> DeleteDowGoalAsync(int id);
        Task<T> GetDowGoalAsync(int id);
        Task<IEnumerable<T>> GetDowGoalsAsync(int Id, bool forceRefresh = false);
    }
}

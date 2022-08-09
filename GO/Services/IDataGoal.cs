using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IDataGoal<T>
    {
        Task<bool> AddGoalAsync(T item);
        Task<bool> UpdateGoalAsync(T item);
        Task<bool> DeleteGoalAsync(int id);
        Task<T> GetGoalAsync(int id);  
        Task<IEnumerable<T>> GetGoalsAsync(int Id, bool forceRefresh = false);
        Task<IEnumerable<T>> GetGoalsAsync(bool forceRefresh = false);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IGoalWeek<T>
    {
        Task<bool> AddGoalWeekAsync(T item);
        Task<bool> UpdateGoalWeekAsync(T item);
        Task<IEnumerable<T>> GetGoalWeeksAsync(bool forceRefresh = false);
    }
}

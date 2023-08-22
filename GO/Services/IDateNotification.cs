using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GO.Services
{
    public interface IDateNotification<T>
    {
        Task<bool> AddNotificationAsync(T item);
        Task<bool> UpdateNotificationAsync(T item);
        Task<bool> DeleteNotificationAsync(int id);
        Task<T> GetNotificationAsync(int id);
        Task<IEnumerable<T>> GetNotificationGoalAsync(int GoalId, bool forceRefresh = false);
        Task<IEnumerable<T>> GetNotificationTaskAsync(int TaskId, bool forceRefresh = false);
        Task<IEnumerable<T>> GetNotificationSubtaskAsync(int SubtaskId, bool forceRefresh = false);

    }
}

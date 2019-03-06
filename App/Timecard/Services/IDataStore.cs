using System.Collections.Generic;
using System.Threading.Tasks;
using Timecard.Models;

namespace Timecard
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        Task<IEnumerable<CostCode>> GetCostCodesAsync();
        Task<IEnumerable<Job>> GetJobsAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Timecard.Models;
using Timecard.Services;

namespace Timecard
{
    public interface IDataStore<T>
    {
        Task<FirebaseUserInfo> AuthenticateUser(GoogleUserInfo googleUserInfo);

        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        Task<IEnumerable<CostCode>> GetCostCodesAsync();
        Task<IEnumerable<Job>> GetJobsAsync();
    }
}

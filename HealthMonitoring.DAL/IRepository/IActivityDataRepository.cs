using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;

namespace HealthMonitoring.DAL.IRepository
{
    public interface IActivityDataRepository :IBaseRepository<ActivityData>
    {
        Task<ActivityData> GetUserDayActivityAsync(string userId, int day);
        Task<IEnumerable<ActivityData>> GetUserActivitiesAsync(string userId);
        Task BulkInsertUserActivitiesAsync(string userId, IEnumerable<ActivityData> activities);
        Task AddrangeAsync(List<ActivityData> batch);
    }
}

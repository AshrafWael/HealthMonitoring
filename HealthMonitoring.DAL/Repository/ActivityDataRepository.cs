using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository
{
    public class ActivityDataRepository : BaseRepository<ActivityData> ,IActivityDataRepository
    {
        private readonly HealthMonitoringContext _dbcontext;

        public ActivityDataRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));

        }

        public async Task BulkInsertUserActivitiesAsync(string userId, IEnumerable<ActivityData> activities)
        {
            var existingActivities = await _dbcontext.ActivityDatas
                .Where(a => a.UserId == userId)
                .ToListAsync();
            _dbcontext.ActivityDatas.RemoveRange(existingActivities); 
            
            foreach (var activity in activities)
            {
                activity.UserId = userId;
                activity.CreatedAt = DateTime.UtcNow;
                activity.UpdatedAt = DateTime.UtcNow;
            }
            await _dbcontext.ActivityDatas.AddRangeAsync(activities);
        }

        public async Task<IEnumerable<ActivityData>> GetUserActivitiesAsync(string userId)
        {
            return await _dbcontext.ActivityDatas
                 .Where(a => a.UserId == userId)
                 .OrderBy(d => d.Day)
                 .ToListAsync();
        }

        public async Task<ActivityData> GetUserDayActivityAsync(string userId, int day)
        {
        return await _dbcontext.ActivityDatas
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Day == day);  
        }
    }
}

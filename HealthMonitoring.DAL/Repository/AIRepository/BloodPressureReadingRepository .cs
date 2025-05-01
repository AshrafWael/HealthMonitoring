using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.IRepository.IAIRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository.AIRepository
{
    public class BloodPressureReadingRepository : BaseRepository<BloodPressureReading>, IBloodPressureReadingRepository
    {
        public BloodPressureReadingRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
        }

        public async Task<List<BloodPressureReading>> GetRecentBloodPressureByUserIdAsync(string userId)
        {
            return await _dbcontext.bloodPressureReadings
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }
        public async Task<BloodPressureReading> GetLatestBloodPressureByUserIdAsync(string userId)
        {
            return await _dbcontext.bloodPressureReadings
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<BloodPressureReading>> GetBloodPressureByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            IQueryable<BloodPressureReading> query = _dbset;
            return await query.Where(h => h.UserId == userId &&
                           h.Timestamp >= startDate &&
                           h.Timestamp <= endDate)
                            .OrderBy(h => h.Timestamp)
                            .ToListAsync();
        }
    }
}

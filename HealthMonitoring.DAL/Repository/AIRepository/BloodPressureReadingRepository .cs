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

        public async Task<List<BloodPressureReading>> GetRecentByUserIdAsync(string userId, int count)
        {
            return await _dbcontext.bloodPressureReadings
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Timestamp)
                .Take(count)
                .ToListAsync();
        }
        public async Task<BloodPressureReading> GetLatestByUserIdAsync(string userId)
        {
            return await _dbcontext.bloodPressureReadings
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}

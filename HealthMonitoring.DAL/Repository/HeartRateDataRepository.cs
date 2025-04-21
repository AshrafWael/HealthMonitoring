using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository
{
    public class HeartRateDataRepository :BaseRepository<HeartRateData> ,IHeartRateDataRepository
    {

        private readonly HealthMonitoringContext _context;
    //    internal readonly DbSet<HeartRateData> _dbset;
        public HeartRateDataRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
            

        }
        public async Task<IEnumerable<HeartRateData>> GetUserHeartRateDataAsync(string userId)
        {

            IQueryable<HeartRateData> query = _dbset;

         return await  query.Where(h => h.UserId == userId)
                .OrderByDescending(h => h.RecordedAt)
                .ToListAsync();
        }

        public async Task<HeartRateData> GetLatestHeartRateAsync(string userId)
        {
            IQueryable<HeartRateData> query = _dbset;
            return await query.Where(h => h.UserId == userId)
                .OrderByDescending(h => h.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HeartRateData>> GetHeartRatesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            IQueryable<HeartRateData> query = _dbset;

            return await query .Where(h => h.UserId == userId &&
                           h.RecordedAt >= startDate &&
                           h.RecordedAt <= endDate)
                            .OrderBy(h => h.RecordedAt)
                            .ToListAsync();
        }
    }
}

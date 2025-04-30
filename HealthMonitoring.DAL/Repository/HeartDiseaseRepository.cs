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

namespace HealthMonitoring.DAL.Repository
{
    public class HeartDiseaseRepository :BaseRepository<HeartDisease> ,IHeartDiseaseRepository
    {
        public HeartDiseaseRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {}
             public async Task<IEnumerable<HeartDisease>> GetUserHeartRateDataAsync(string userId)
        { 

            IQueryable<HeartDisease> query = _dbset;

            return await query.Where(h => h.UserId == userId)
                   .OrderByDescending(h => h.RecordedAt)
                   .ToListAsync();
        }

        public async Task<HeartDisease> GetLatestHeartRateAsync(string userId)
        {
            IQueryable<HeartDisease> query = _dbset;
            return await query.Where(h => h.UserId == userId)
                .OrderByDescending(h => h.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HeartDisease>> GetHeartRatesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            IQueryable<HeartDisease> query = _dbset;

            return await query.Where(h => h.UserId == userId &&
                           h.RecordedAt >= startDate &&
                           h.RecordedAt <= endDate)
                            .OrderBy(h => h.RecordedAt)
                            .ToListAsync();
        }

    }
    }


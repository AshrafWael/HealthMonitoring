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
    public class SensorDataSetRepository : BaseRepository<SensorDataPoint>, ISensorDataSetRepository
    {
        public SensorDataSetRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
        }

        public async Task<List<SensorDataPoint>> GetByUserIdAsync(string userId, int count,int skip  = 0 )
        {
            //return await _dbcontext.sensorDataPoints
            //    .Where(d => d.UserId == userId)
            //    .Take(count)
            //    .ToListAsync();

            // Use AsNoTracking for read-only operations
            return await _dbcontext.sensorDataPoints
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Timestamp) // Assuming you want the most recent data
                .Skip(skip)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<SensorDataPoint>> GetLatestBatchByUserIdAsync(string userId)
        {


            return await _dbcontext.sensorDataPoints
                .Where(d => d.UserId == userId  )
                .OrderBy(d => d.Id)
                .ToListAsync();
        }

        public async Task<List<SensorDataPoint>> GetByUserIdBatchedAsync(string userId, int batchSize, int totalNeeded)
        {
            var result = new List<SensorDataPoint>();
            int processedCount = 0;

            while (processedCount < totalNeeded)
            {
                var batch = await _dbcontext.sensorDataPoints
                    .AsNoTracking()
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.Timestamp)
                    .Skip(processedCount)
                    .Take(batchSize)
                    .ToListAsync();

                if (batch.Count == 0)
                    break;

                result.AddRange(batch);
                processedCount += batch.Count;

                if (batch.Count < batchSize) // No more data available
                    break;
            }

            return result.Take(totalNeeded).ToList();
        }
    

        public async Task Addrange(List<SensorDataPoint> batch)
        {
            await _dbset.AddRangeAsync(batch);
        }
    }
}

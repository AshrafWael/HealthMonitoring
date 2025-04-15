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
    public class SensorDataSetRepository : BaseRepository<SensorDataSet>, ISensorDataSetRepository
    {
        public SensorDataSetRepository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
        }
        // get spefic amount of data set by user id 
        public async Task<List<SensorDataSet>> GetByUserIdAsync(string userId, int count,int skip =0 )
        {
            return await _dbcontext.sensorDataSets
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.Timestamp) 
                .Skip(skip *count)
                .Take(count)
                .ToListAsync();
        }
        //Get All data set by user id
        public async Task<List<SensorDataSet>> GetSensordataByUserIdAsync(string userId)
        {
            return await _dbcontext.sensorDataSets
                .AsNoTracking()
                .Where(d => d.UserId == userId  )
                .OrderBy(d => d.Id)
                .ToListAsync();
        }
        public async Task<List<SensorDataSet>> GetByUserIdBatchedAsync(string userId, int batchSize, int totalNeeded)
        {
            var result = new List<SensorDataSet>();
            int processedCount = 0;

            while (processedCount < totalNeeded)
            {
                var batch = await _dbcontext.sensorDataSets
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
        public async Task AddrangeAsync(List<SensorDataSet> sensorDatas)
        {
            await _dbset.AddRangeAsync(sensorDatas);
        }

    }
}

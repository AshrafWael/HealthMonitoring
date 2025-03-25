using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.DAL.IRepository.IAIRepository
{
    public interface ISensorDataSetRepository :IBaseRepository<SensorDataPoint>
    {
        Task<List<SensorDataPoint>> GetByUserIdAsync(string userId,int count, int skip = 0);
        public  Task<List<SensorDataPoint>> GetByUserIdBatchedAsync(string userId, int batchSize, int totalNeeded);
        public Task Addrange(List<SensorDataPoint> batch);

    }
}

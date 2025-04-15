using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.DAL.IRepository.IAIRepository
{
    public interface ISensorDataSetRepository :IBaseRepository<SensorDataSet>
    {
         Task<List<SensorDataSet>> GetByUserIdAsync(string userId,int count, int skip =0);
         Task<List<SensorDataSet>> GetByUserIdBatchedAsync(string userId, int batchSize, int totalNeeded);
        Task AddrangeAsync(List<SensorDataSet> sensorDatas);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.DAL.IRepository
{
    public interface IHeartRateDataRepository :IBaseRepository<HeartRateData>
    {

        Task<IEnumerable<HeartRateData>> GetUserHeartRateDataAsync(string userId);
        Task<HeartRateData> GetLatestHeartRateAsync(string userId);
        Task<IEnumerable<HeartRateData>> GetHeartRatesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    }
}

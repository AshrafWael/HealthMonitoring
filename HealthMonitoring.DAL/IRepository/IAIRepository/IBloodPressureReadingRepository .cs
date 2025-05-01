using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.DAL.IRepository.IAIRepository
{
    public interface IBloodPressureReadingRepository :IBaseRepository<BloodPressureReading>
    {
        Task<List<BloodPressureReading>> GetRecentBloodPressureByUserIdAsync(string userId);
        Task<BloodPressureReading> GetLatestBloodPressureByUserIdAsync(string userId);
        public Task<IEnumerable<BloodPressureReading>> GetBloodPressureByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

    }
}

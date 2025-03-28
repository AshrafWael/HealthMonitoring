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
        Task<List<BloodPressureReading>> GetRecentByUserIdAsync(string userId, int count);
        Task<BloodPressureReading> GetLatestByUserIdAsync(string userId);

    }
}

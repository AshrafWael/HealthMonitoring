using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;

namespace HealthMonitoring.DAL.IRepository.IAIRepository
{
    public interface IHeartDiseaseRepository : IBaseRepository<HeartDisease>
    {
        Task<IEnumerable<HeartDisease>> GetUserHeartRateDataAsync(string userId);
        Task<HeartDisease> GetLatestHeartRateAsync(string userId);
        Task<IEnumerable<HeartDisease>> GetHeartRatesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    }
}

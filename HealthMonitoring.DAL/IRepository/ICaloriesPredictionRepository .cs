using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.IRepository
{
    public interface ICaloriesPredictionRepository :IBaseRepository<CaloriesPrediction>
    {
        Task<CaloriesPrediction> GetUserDayPredictionAsync(string userId, int day);
        Task<IEnumerable<CaloriesPrediction>> GetUserPredictionsAsync(string userId);
        Task<CaloriesPrediction> GetLatestCaloriesPredictionByUserIdAsync(string userId);
        public Task<IEnumerable<CaloriesPrediction>> GetCaloriesPredictionByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    }
}

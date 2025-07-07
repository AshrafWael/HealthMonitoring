using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.Dtos.BloodPressureDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.IServices
{
    public interface ICaloriesPredictionService
    {
        Task<CaloriesPredictionResponseDto> PredictCaloriesForUserDayAsync(CaloriesPredictionRequestDto request);
        public  Task<int> ImportBulkActivityDataAsync(UserActivityJsonDto data, string userId);
        public  Task<int> ImportBulkActivityDataFromJsonFileAsync(string jsonFilePath, string userId);
        Task<IEnumerable<CaloriesPredictionResponseDto>> GetUserPredictionsAsync(string userId);
        public  Task UpdateUserWeight(string id, double weight);
        public Task<CaloriesPredictionResponseDto> GetLatestReadingAsync(string userId);
        public Task<IEnumerable<CaloriesPredictionResponseDto>> GetReadingsByDateRangeAsync(string userId, DateTime start, DateTime end);
    }
}

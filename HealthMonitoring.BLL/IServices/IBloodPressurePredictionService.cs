using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.BloodPressureDto;
using HealthMonitoring.DAL.Data.Models.AIModels;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.IServices
{
    public interface IBloodPressurePredictionService
    {
        public Task<BloodPressurePrediction> PredictBloodPressure(string userid,int batchNumber);
        public  BloodPressureCategory DetermineBloodPressureCategory(double systolic, double diastolic);
        public Task StoreBloodPressurePrediction(string userId, double systolic, double diastolic, BloodPressureCategory category);
        public Task<List<BloodPressurReadDto>> GetRecentReadingsAsync(string userId );
        public Task<BloodPressurReadDto> GetLatestReadingAsync(string userId);
        public Task<IEnumerable<BloodPressurReadDto>> GetReadingsByDateRangeAsync(string userId, DateTime start, DateTime end);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.HealthInformationDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
using HealthMonitoring.DAL.Data.Models;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.IServices
{
    public interface IHeartDiseaseService
    {
        public Task<HeartDiseaseReadDto> PredictHeartDisease(string userid, int batchNumber);
        public Task StoreHeartDisease(string userId, string heartdisease);
        Task<IEnumerable<HeartDiseasesReadingDto>> GetUserHeartDiseaseDataAsync(string userId);
        Task<HeartDiseasesReadingDto> GetLatestHeartDiseaseAsync(string userId);
        Task<IEnumerable<HeartDiseasesReadingDto>> GetHeartDiseaseByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

    }
}

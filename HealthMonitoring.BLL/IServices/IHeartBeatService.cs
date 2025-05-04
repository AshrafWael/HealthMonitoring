using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
using HealthMonitoring.DAL.Data.Models.AIModels;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.IServices
{
    public interface IHeartBeatService
    {
        public Task<HeartRateDataReadDto> CalculateHeartBeat(string userid, int batchNumber);
        public HeartRateCategory DetermineHeartBeatStatus(int heartrate);
        public Task StoreHeartRate(string userId, int heartrate,HeartRateCategory Category);
        public Task<IEnumerable<HeartRateReadingDto>> GetUserHeartRateDataAsync(string userId);
        public Task<HeartRateReadingDto> GetLatestHeartRateAsync(string userId);
        public Task<IEnumerable<HeartRateReadingDto>> GetHeartRatesByDateRangeAsync(string userId, DateTime start, DateTime end);


    }
}

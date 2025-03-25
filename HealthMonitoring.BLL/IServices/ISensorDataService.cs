using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.DAL.Data.Models.AIModels;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.IServices
{
    public interface ISensorDataService
    {
       

        Task<int> ImportBulkSensorDataAsync(DataPointDto data, string userId);
        Task<int> ImportBulkSensorDataFromJsonFileAsync(string jsonFilePath, string userId);
        Task<bool> CacheDatasetAsync(DataPointDto data, string cacheKey);
        Task<DataPointDto> GetCachedDatasetAsync(string cacheKey);

        Task<List<DataPointDto>> GetDataSetByUser( string userId);
        public  Task<List<DataPointDto>> GetDataSetByUserChunked(string userId, int chunkSize = 250);

    }
}

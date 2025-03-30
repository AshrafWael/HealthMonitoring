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
       

        Task<int> ImportBulkSensorDataAsync(DataSetDto data, string userId);
        Task<int> ImportBulkSensorDataFromJsonFileAsync(string jsonFilePath, string userId);
        Task<bool> CacheDatasetAsync(DataSetDto data, string cacheKey);
        Task<DataSetDto> GetCachedDatasetAsync(string cacheKey);
        Task<List<DataSetDto>> GetDataSetByUser( string userId);
        Task<List<DataSetDto>> GetDataSetByUserChunked(string userId, int chunkSize = 250);

    }
}

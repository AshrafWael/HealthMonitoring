using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Dtos.HealthInformationDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
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
        Task<List<DataSetDto>> GetDataSetByUser( string userId, int batchNumber);
        Task<List<DataSetDto>> GetDataSetByUserChunked(string userId, int chunkSize = 250);
        public Task<bool> DeleteDataSetByUserId(string userid);
        public Task<List<HeartRateRequstDto>> GetecgDataSetByUser(string userId, int batchNumber);
        public Task<List<HeartDiseaseRequstDto>> GetecgDataSetByUserAsync(string userId, int batchNumber);




    }
}

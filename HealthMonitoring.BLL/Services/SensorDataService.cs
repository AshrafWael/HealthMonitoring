using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Dtos.HealthInformationDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.Services
{
    public class SensorDataService : ISensorDataService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SensorDataService> _logger;
        private List<SensorDataSet> _cachedDataSet;
        private readonly Dictionary<string, Timer> _monitoringTimers = new();
        private readonly object _lockObject = new();
        private const int BATCH_SIZE = 500;

        public SensorDataService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<SensorDataService> logger ,
             IMemoryCache cache)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
           _cache = cache;
        }
        public async Task<int> ImportBulkSensorDataAsync(DataSetDto data, string userId)
        {
            try
            {
                _logger.LogInformation("Starting bulk import of sensor data for user {UserId}", userId);

                // Validate data
                if (data == null || data.PPG == null || data.ABP == null || data.ECG == null)
                {
                    throw new ArgumentException("Invalid sensor data provided");
                }

                // Ensure all arrays have the same length
                if (data.PPG.Count != data.ABP.Count || data.PPG.Count != data.ECG.Count)
                {
                    throw new ArgumentException("Sensor data arrays must have the same length");
                }
                int totalReadings = data.PPG.Count;
                _logger.LogInformation("Processing {Count} readings for user {UserId}", totalReadings, userId);

                int totalImported = 0;
                // Process in batches to avoid memory issues

                for (int i = 0; i < totalReadings; i += BATCH_SIZE)
                {
                    int batchSize = Math.Min(BATCH_SIZE, totalReadings - i);
                   // var mappedModel = _mapper.Map<SensorDataSet>(data);
                    var batch = new List<SensorDataSet>();

                    for (int j = 0; j < batchSize; j++)
                    {
                        int index = i + j;
                        batch.Add(new SensorDataSet
                        {
                            UserId = userId,
                            Timestamp = DateTime.UtcNow.AddMilliseconds(-1 * (totalReadings - index)), // Create sequential timestamps
                            PPG = data.PPG[index],
                            ABP = data.ABP[index],
                            ECG = data.ECG[index]
                        });
                    }

                    await _unitOfWork.sensorDataSet.AddrangeAsync(batch);
                    await _unitOfWork.SaveChangesAsync();

                    totalImported += batch.Count;

                    _logger.LogInformation("Imported batch {BatchNumber} ({Count} readings) for user {UserId}",
                        (i / BATCH_SIZE) + 1, batch.Count, userId);
                }

                // Clear any cache for this user to ensure fresh data
                InvalidateUserCache(userId);
                _logger.LogInformation("Completed bulk import of {Count} sensor readings for user {UserId}",
                    totalImported, userId);

                return totalImported;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk import of sensor data for user {UserId}", userId);
                throw;
            }
        }
        public async Task<int> ImportBulkSensorDataFromJsonFileAsync(string jsonFilePath, string userId)
        {
            try
            {
                _logger.LogInformation("Starting import from JSON file for user {UserId}", userId);

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("JSON file not found", jsonFilePath);
                }
                // Read the file
                string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                // Deserialize
                var data = JsonSerializer.Deserialize<DataSetDto>(jsonContent);
                // Import the data
                return await ImportBulkSensorDataAsync(data, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing sensor data from JSON file for user {UserId}", userId);
                throw;
            }
        }
        public async Task<bool> CacheDatasetAsync(DataSetDto data, string cacheKey)
        {
            try
            {
                // Set the cache with a sliding expiration of 30 minutes
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSize(1) // For memory management
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2));
                _cache.Set(cacheKey, data, cacheOptions);
                _logger.LogInformation("Dataset cached with key {CacheKey}", cacheKey);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching dataset with key {CacheKey}", cacheKey);
                return false;
            }
        }
        public async Task<DataSetDto> GetCachedDatasetAsync(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out DataSetDto cachedData))
            {
                _logger.LogInformation("Retrieved cached dataset with key {CacheKey}", cacheKey);
                return cachedData;
            }

            _logger.LogInformation("No cached dataset found with key {CacheKey}", cacheKey);
            return null;
        }
        private void InvalidateUserCache(string userId)
        {
            // Remove any relevant cache entries for this user
            // This is simplified - you'd need to track and remove all relevant cache keys
            var keysToInvalidate = new[]
            {
            $"user_{userId}_dataset",
            $"user_{userId}_readings_",
            $"user_{userId}_has_sufficient_data"
        };

            foreach (var key in keysToInvalidate)
            {
                _cache.Remove(key);
            }
        }
        public async  Task<List<DataSetDto>> GetDataSetByUser(string userId, int batchNumber )
        {
            string cacheKey = $"user_{userId}_dataset";
            // Check if data is in cache
            if (_cache.TryGetValue(cacheKey, out List<DataSetDto> cachedData))
            {
                _logger.LogInformation($"Cache hit for user data: {userId}");
                return cachedData;
            }
            _logger.LogInformation($"Fetching data for user: {userId}");
                  var data = await _unitOfWork.sensorDataSet.GetByUserIdAsync(userId, 1250, batchNumber);      
                var datasetDto = _mapper.Map<List<DataSetDto>>(data);       
                  return datasetDto;

            /*
            // Cache data with a sliding expiration
            //var cacheOptions = new MemoryCacheEntryOptions()
            //    .SetSlidingExpiration(TimeSpan.FromMinutes(10)).SetSize(1);
            //_cache.Set(cacheKey, datasetDto, cacheOptions);
            */

        }
        public async Task<List<HeartRateRequstDto>> GetecgDataSetByUser(string userId, int batchNumber)
        {
            string cacheKey = $"user_{userId}_dataset";
            // Check if data is in cache
            if (_cache.TryGetValue(cacheKey, out List<HeartRateRequstDto> cachedData))
            {
                _logger.LogInformation($"Cache hit for user data: {userId}");
                return cachedData;
            }
            _logger.LogInformation($"Fetching data for user: {userId}");

            var data = await _unitOfWork.sensorDataSet.GetECGByUserIdAsync(userId, 1250, batchNumber);
            var datasetDto = _mapper.Map<List<HeartRateRequstDto>>(data);
            /*
           // Cache data with a sliding expiration
           //var cacheOptions = new MemoryCacheEntryOptions()
           //    .SetSlidingExpiration(TimeSpan.FromMinutes(10)).SetSize(1);
           //_cache.Set(cacheKey, datasetDto, cacheOptions);
           */
            return datasetDto;
        }
        public async Task<List<HeartDiseaseRequstDto>> GetecgDataSetByUserAsync(string userId, int batchNumber)
        {
            string cacheKey = $"user_{userId}_dataset";
            // Check if data is in cache
            if (_cache.TryGetValue(cacheKey, out List<HeartDiseaseRequstDto> cachedData))
            {
                _logger.LogInformation($"Cache hit for user data: {userId}");
                return cachedData;
            }
            _logger.LogInformation($"Fetching data for user: {userId}");

            var data = await _unitOfWork.sensorDataSet.GetECGByUserIdAsync(userId, 187, batchNumber);
            var datasetDto = _mapper.Map<List<HeartDiseaseRequstDto>>(data);
            /*
           // Cache data with a sliding expiration
           //var cacheOptions = new MemoryCacheEntryOptions()
           //    .SetSlidingExpiration(TimeSpan.FromMinutes(10)).SetSize(1);
           //_cache.Set(cacheKey, datasetDto, cacheOptions);
           */
            return datasetDto;
        }
        public async Task<List<DataSetDto>> GetDataSetByUserChunked(string userId, int chunkSize = 250)
        {
            string cacheKey = $"user_data_{userId}";

            // Check if data is in cache
            if (_cache.TryGetValue(cacheKey, out List<DataSetDto> cachedData))
            {
                _logger.LogInformation($"Cache hit for user data: {userId}");
                return cachedData;
            }

            _logger.LogInformation($"Fetching chunked data for user: {userId}");

            const int totalNeeded = 1250;
            var result = new List<DataSetDto>();

            // Process in chunks
            for (int offset = 0; offset < totalNeeded; offset += chunkSize)
            {
                int currentChunkSize = Math.Min(chunkSize, totalNeeded - offset);
                var chunk = await _unitOfWork.sensorDataSet.GetByUserIdAsync(userId, currentChunkSize, offset);
                var chunkDto = _mapper.Map<List<DataSetDto>>(chunk);
                result.AddRange(chunkDto);

                if (chunk.Count < currentChunkSize) // No more data available
                    break;
            }

            // Cache the aggregated result
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, result, cacheOptions);

            return result;
        }
        public async Task<bool> DeleteDataSetByUserId(string userid)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userid);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with userid {userid} not found.");
            }
            var sensorData = await _unitOfWork.sensorDataSet.FindAllAsync(u => u.UserId == userid, null, null);
            if (sensorData == null)
            {
                throw new KeyNotFoundException($"No sensor data found for user {userid}");
            }
            try
            {
                var mappedData = _mapper.Map<SensorDataSet>(sensorData);
                // Delete the sensor data
              await  _unitOfWork.sensorDataSet.RemoveAsync(mappedData);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deleted sensor data for user {UserId}", userid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sensor data for user {UserId}", userid);
                return false;
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vonage.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.Services
{
    public class HeartBeatService : IHeartBeatService
    {
        private readonly HttpClient _httpClient;
        private readonly ISensorDataService _sensorDataService;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _serviceURL;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public HeartBeatService(HttpClient httpClient, IConfiguration configuration
            , ISensorDataService sensorDataService, ILogger<BloodPressurePredictionService> logger
            , IUnitOfWork unitOfWork, IMemoryCache cache,IMapper mapper)
        {
            _httpClient = httpClient;
            _sensorDataService = sensorDataService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _serviceURL = configuration.GetValue<string>("AIModel:HeartRateUrl")!;
            _cache = cache;
           _mapper = mapper;
        }
        public async Task<HeartRateDataReadDto> CalculateHeartBeat(string userId, int batchNumber)
        {
            // Fetch latest 1250 sensor data records

            var latestData = await _sensorDataService.GetecgDataSetByUser(userId, batchNumber);


            if (!latestData.Any())
            {
                throw new Exception("No sensor data found for the user.");
            }

            // Prepare request payload
            var requestDto = new
            {
                ecg  = latestData.Select(d => d.ECG).ToList()
            };

            string requestJson = JsonConvert.SerializeObject(requestDto);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {
                _logger.LogInformation($"Sending request to AI Model for user {userId}");

                HttpResponseMessage response = await _httpClient.PostAsync($"{_serviceURL}/calculate-heart-rate", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI Model returned an error: {response.StatusCode}, Response: {errorResponse}");
                    throw new Exception($"AI Model error: {errorResponse}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var prediction = JsonConvert.DeserializeObject<HeartRateDataReadDto>(jsonResponse);

                _logger.LogInformation($"Received AI prediction for user {userId}");
                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error communicating with AI Model for user {userId}");
                throw new Exception("Failed to get blood pressure prediction from AI Model", ex);
            }
        }
        public HeartRateCategory DetermineHeartBeatStatus(int heartrate)
        {
            if (heartrate < 120 && heartrate < 80)
                return HeartRateCategory.Normal;
            else if (heartrate < 120 && heartrate < 100)
                return HeartRateCategory.High;
            else
                return HeartRateCategory.Normal;
        }
        public async Task StoreHeartRate(string userId, int heartrate,HeartRateCategory Category)
        {
            try
            {
                var reading = new HeartRateData
                {
                    UserId = userId,
                    RecordedAt = DateTime.UtcNow,
                    HeartRate = heartrate,
                    Category = Category
                };            
                await _unitOfWork.HeartRateDatas.CreateAsync(reading);
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store blood pressure prediction");
                // Don't throw - we want to return the prediction even if storage fails
            }
        }
        public async Task<IEnumerable<HeartRateReadingDto>> GetHeartRatesByDateRangeAsync(string userId, DateTime start, DateTime end)
        {
           var data=  await _unitOfWork.HeartRateDatas.GetHeartRatesByDateRangeAsync(userId, start, end);
            var mappeddata = _mapper.Map<IEnumerable<HeartRateReadingDto>>(data);
            return mappeddata;
        }
        public async Task<HeartRateReadingDto> GetLatestHeartRateAsync(string userId)
        {
            var data = await _unitOfWork.HeartRateDatas.GetLatestHeartRateAsync(userId);
            var mappeddata = _mapper.Map<HeartRateReadingDto>(data);
            return mappeddata;
        }
        public async Task<IEnumerable<HeartRateReadingDto>> GetUserHeartRateDataAsync(string userId)
        {
            var data = await _unitOfWork.HeartRateDatas.GetUserHeartRateDataAsync(userId);
            var mappeddata = _mapper.Map<IEnumerable<HeartRateReadingDto>>(data);
            return mappeddata;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.APIRequst;
using HealthMonitoring.BLL.ApiResponse;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Dtos.BloodPressureDto;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static HealthMonitoring.BLL.StaticData.StaticData;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.BLL.Services
{
    public class BloodPressurePredictionService : IBloodPressurePredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly ISensorDataService _sensorDataService;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _serviceURL;

        public BloodPressurePredictionService(HttpClient httpClient, IConfiguration configuration
            , ISensorDataService sensorDataService, ILogger<BloodPressurePredictionService> logger
            , IUnitOfWork unitOfWork,IMapper mapper)
        {
            _httpClient = httpClient;
            _sensorDataService = sensorDataService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceURL = configuration.GetValue<string>("AIModel:BloodPressurUrl")!;
        }
        public async Task<BloodPressurePrediction> PredictBloodPressure(string userId,int batchNumber)
        {
            // Fetch latest 1250 sensor data records
            
                var latestData = await _sensorDataService.GetDataSetByUser(userId , batchNumber);
                 

            if (!latestData.Any())
            {
                throw new Exception("No sensor data found for the user.");
            }

            // Prepare request payload
            var requestDto = new
            {
                PPG = latestData.Select(d => d.PPG).ToList(),
                ECG = latestData.Select(d => d.ECG).ToList(),
                ABP = latestData.Select(d => d.ABP).ToList()
            };

            string requestJson = JsonConvert.SerializeObject(requestDto);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {
                _logger.LogInformation($"Sending request to AI Model for user {userId}");

                HttpResponseMessage response = await _httpClient.PostAsync($"{_serviceURL}/predict", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI Model returned an error: {response.StatusCode}, Response: {errorResponse}");
                    throw new Exception($"AI Model error: {errorResponse}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var prediction = JsonConvert.DeserializeObject<BloodPressurePrediction>(jsonResponse);

                _logger.LogInformation($"Received AI prediction for user {userId}");
                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error communicating with AI Model for user {userId}");
                throw new Exception("Failed to get blood pressure prediction from AI Model", ex);
            }
        }

        //public async Task<BloodPressurePrediction> PredictBloodPressure(string userid)
        //{
        //    // Fetch latest 1250 sensor data records
        //    var latestData = await _sensorDataService.GetDataSetByUser(userId);
        //    if (!latestData.Any())
        //    {
        //        throw new Exception("No sensor data found for the user.");
        //    }

        //    // Prepare request payload
        //    var requestDto = new
        //    {
        //        PPG = latestData.Select(d => d.PPG).ToList(),
        //        ECG = latestData.Select(d => d.ECG).ToList(),
        //        ABP = latestData.Select(d => d.ABP).ToList()
        //    };

        //    var response = await SendAsync<BloodPressurePrediction>(new APIRequest()
        //    {
        //        ApiType = ApiType.POST,
        //        Data = requestDto,
        //        ApiUrl = _serviceURL + "/predict"
        //    });

        //    return response;

        //}
        public BloodPressureCategory DetermineBloodPressureCategory(double systolic, double diastolic)
        {
            if (systolic < 120 && diastolic < 80)
                return BloodPressureCategory.Normal;
            else if ((systolic >= 120 && systolic <= 129) && diastolic < 80)
                return BloodPressureCategory.Elevated;
            else if ((systolic >= 130 && systolic <= 139) || (diastolic >= 80 && diastolic <= 89))
                return BloodPressureCategory.HypertensionStage1;
            else if (systolic >= 140 || diastolic >= 90)
                return BloodPressureCategory.HypertensionStage2;
            else if (systolic > 180 || diastolic > 120)
                return BloodPressureCategory.HypertensiveCrisis;
            else
                return BloodPressureCategory.HypertensiveCrisis;
        }

        public async Task StoreBloodPressurePrediction(string userId, double systolic, double diastolic, BloodPressureCategory category)
        {
            try
            {
                var reading = new BloodPressureReading
                {
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    sbp = systolic,
                    dbp = diastolic,
                    Category = category
                };

                await _unitOfWork.bloodPressureReading.CreateAsync(reading);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store blood pressure prediction");
                // Don't throw - we want to return the prediction even if storage fails
            }
        }

        public async Task<List<BloodPressurReadDto>> GetRecentReadingsAsync(string userId )
        {

           var data = await _unitOfWork.bloodPressureReading.GetRecentBloodPressureByUserIdAsync(userId);
            var mappeddata = _mapper.Map<List<BloodPressurReadDto>>(data);
            return mappeddata;
        }
        public async Task<BloodPressurReadDto> GetLatestReadingAsync(string userId)
        {
         var data=  await _unitOfWork.bloodPressureReading.GetLatestBloodPressureByUserIdAsync(userId);
            var mappeddata = _mapper.Map<BloodPressurReadDto>(data);
            return mappeddata;
        }
        
        public async Task<IEnumerable<BloodPressurReadDto>> GetReadingsByDateRangeAsync(string userId, DateTime start, DateTime end)
        {
          var data =  await _unitOfWork.bloodPressureReading.GetBloodPressureByDateRangeAsync(userId, start, end);
            var mappeddata = _mapper.Map<IEnumerable<BloodPressurReadDto>>(data);
            return mappeddata;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.BLL.APIRequst;
using HealthMonitoring.BLL.ApiResponse;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models.AIModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static HealthMonitoring.BLL.StaticData.StaticData;

namespace HealthMonitoring.BLL.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly HttpClient _httpClient;
        private readonly ISensorDataService _sensorDataService;
        private readonly ILogger _logger;
        private readonly string _serviceURL;
     //   private readonly SemaphoreSlim _semaphore;

        public AIModelService(HttpClient httpClient, IConfiguration configuration
            ,ISensorDataService sensorDataService , ILogger<AIModelService> logger) 
        {
            _httpClient = httpClient;
            _sensorDataService = sensorDataService;
            _logger = logger;
            _serviceURL = configuration.GetValue<string>("AIModel:BaseUrl")!;
            // Limit concurrent requests to the AI model
           // _semaphore = new SemaphoreSlim(3, 3); // Allow 3 concurrent requests

        }

        public async Task<BloodPressurePrediction> PredictBloodPressure(string userId)
        {
            // Fetch latest 1250 sensor data records
            var latestData = await _sensorDataService.GetDataSetByUser(userId);
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
        /*
        public  async Task<BloodPressurePrediction> PredictBloodPressure(string userid)
        {
            // Fetch 1250 latest data records
            var latestData = await _sensorDataService.GetDataSetByUser(userid);

            if (!latestData.Any())
            {
                throw new Exception("No sensor data found for the user.");
            }

            // Convert to DTO
            var requestDto = new 
            {
                PPG = latestData.Select(d => d.PPG).ToList(),
                ABP = latestData.Select(d => d.ABP).ToList(),
                ECG = latestData.Select(d => d.ECG).ToList()
            };

            var response = await SendAsync<BloodPressurePredictionResponse>(new APIRequest()
            {
                ApiType = ApiType.POST,
                Data = requestDto,
                ApiUrl =_serviceURL + "/predict"
            });

            return response.Prediction;

        }

        */

    }
}

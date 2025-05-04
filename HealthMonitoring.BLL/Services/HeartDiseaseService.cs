using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.Dtos.HealthInformationDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vonage.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HealthMonitoring.BLL.Services
{
    public class HeartDiseaseService : IHeartDiseaseService
    {
        private readonly HttpClient _httpClient;
        private readonly ISensorDataService _sensorDataService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _serviceURL;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        public HeartDiseaseService(HttpClient httpClient, IConfiguration configuration
            , ISensorDataService sensorDataService
            , IUnitOfWork unitOfWork, IMemoryCache cache,IMapper mapper)
        {
            _httpClient = httpClient;
            _sensorDataService = sensorDataService;
            _unitOfWork = unitOfWork;
            _serviceURL = configuration.GetValue<string>("AIModel:HeartDisease")!;
            _cache = cache;
            _mapper = mapper;
        }
        public async Task<HeartDiseaseReadDto> PredictHeartDisease(string userid, int batchNumber)
        {

            var latestData = await _sensorDataService.GetecgDataSetByUserAsync(userid, batchNumber);


            if (!latestData.Any())
            {
                throw new Exception("No sensor data found for the user.");
            }

            // Prepare request payload
            var requestDto = new
            {
                ECG = latestData.Select(d => d.ECG).ToList()
            };

            string requestJson = JsonConvert.SerializeObject(requestDto);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {

                HttpResponseMessage response = await _httpClient.PostAsync($"{_serviceURL}/predict-ecg", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"AI Model error: {errorResponse}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var prediction = JsonConvert.DeserializeObject<HeartDiseaseReadDto>(jsonResponse);

                return prediction;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get Heart Diseas prediction from AI Model", ex);
            }
        }

        public async Task StoreHeartDisease(string userId, string heartdisease)
        {
            try
            {
                var reading = new HeartDisease
                {
                    UserId = userId,
                    RecordedAt = DateTime.UtcNow,
                    Diseases = heartdisease
                };
             
                await _unitOfWork.HeartDiseases.CreateAsync(reading);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                // Don't throw - we want to return the prediction even if storage fails
            }
        }

        public async Task<IEnumerable<HeartDiseasesReadingDto>> GetUserHeartDiseaseDataAsync(string userId)
        {
         var data = await  _unitOfWork.HeartDiseases.GetUserHeartDiseaseDataAsync(userId);
            var mappeddata = _mapper.Map<IEnumerable<HeartDiseasesReadingDto>>(data);
            return mappeddata;
        }
        public async Task<HeartDiseasesReadingDto> GetLatestHeartDiseaseAsync(string userId)
        {
            var data = await _unitOfWork.HeartDiseases.GetLatestHeartDiseaseAsync(userId);
            var mappeddata = _mapper.Map<HeartDiseasesReadingDto>(data);
            return mappeddata;

        }
        public async Task<IEnumerable<HeartDiseasesReadingDto>> GetHeartDiseaseByDateRangeAsync(string userId, DateTime start, DateTime end)
        {

            var data = await _unitOfWork.HeartDiseases.GetHeartDiseaseByDateRangeAsync(userId, start, end);
            var mappeddata = _mapper.Map<IEnumerable<HeartDiseasesReadingDto>>(data);
            return mappeddata;
        }
    }
}


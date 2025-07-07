using AutoMapper;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Services
{
    public class ActivityDataServices : IActivityDataServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ActivityDataServices> _logger;

        public ActivityDataServices(HttpClient httpClient, IConfiguration configuration, ILogger<ActivityDataServices> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AiModelResponseDto> PredictCaloriesAsync(AiModelRequestDto request)
        {
            try
            {
                var aiModelUrl = _configuration["AIModel:ActivityData"] + "/predict";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to AI model: {Url}", aiModelUrl);
                _logger.LogDebug("Request payload: {Payload}", jsonContent);

                var response = await _httpClient.PostAsync(aiModelUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AiModelResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    _logger.LogInformation("AI model prediction successful: {Calories}", result?.PredictedCalories);
                    return result;
                }
                else
                {
                    _logger.LogError("AI model request failed with status: {StatusCode}", response.StatusCode);
                    return new AiModelResponseDto
                    {
                        Success = false,
                        Message = $"AI model request failed with status: {response.StatusCode}",
                        PredictedCalories = 0
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AI model service");
                return new AiModelResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    PredictedCalories = 0
                };
            }
        }
    
    }
}

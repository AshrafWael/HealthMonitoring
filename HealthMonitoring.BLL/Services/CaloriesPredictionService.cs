using AutoMapper;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.Dtos.BloodPressureDto;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Services
{
    public class CaloriesPredictionService : ICaloriesPredictionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityDataServices _aiModelService;
        private readonly IMapper _mapper;
        private readonly ILogger<CaloriesPredictionService> _logger;
                private const int BATCH_SIZE = 500;

        public CaloriesPredictionService(
            IUnitOfWork unitOfWork,
            IActivityDataServices aiModelService,
            IMapper mapper,
            ILogger<CaloriesPredictionService> logger)
        {
            _unitOfWork = unitOfWork;
            _aiModelService = aiModelService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CaloriesPredictionResponseDto> PredictCaloriesForUserDayAsync(CaloriesPredictionRequestDto request)
        {
            try
            {
                _logger.LogInformation("Starting calories prediction for user {UserId}, day {Day}", request.UserId, request.Day);

                // 1. Get user
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new ArgumentException($"User with ID {request.UserId} not found");
                }

                // 2. Get daily activity data for the specific day
                var dailyActivity = await _unitOfWork.ActivityDatas.GetUserDayActivityAsync(request.UserId, request.Day);
                if (dailyActivity == null)
                {
                    throw new ArgumentException($"No activity data found for user {request.UserId} on day {request.Day}");
                }

                // 3. Update user weight with the weight from daily activity data
                if (Math.Abs((double)(user.WeightKg - dailyActivity.WeightKg)) > 0.01) // Only update if different
                {
                    await _unitOfWork.Users.UpdateUserWeightAsync(request.UserId, dailyActivity.WeightKg);
                    _logger.LogInformation("Updated user {UserId} weight from {OldWeight} to {NewWeight}",
                        request.UserId, user.WeightKg, dailyActivity.WeightKg);
                }

                // 4. Prepare AI model request
                var aiRequest = new AiModelRequestDto
                {
                    TotalSteps = dailyActivity.TotalSteps,
                    VeryActiveMinutes = dailyActivity.VeryActiveMinutes,
                    FairlyActiveMinutes = dailyActivity.FairlyActiveMinutes,
                    LightlyActiveMinutes = dailyActivity.LightlyActiveMinutes,
                    SedentaryMinutes = dailyActivity.SedentaryMinutes,
                    TotalMinutesAsleep = dailyActivity.TotalMinutesAsleep,
                    WeightKg = dailyActivity.WeightKg
                };

                // 5. Call AI model
                var aiResponse = await _aiModelService.PredictCaloriesAsync(aiRequest);
                if (!aiResponse.Success)
                {
                    throw new Exception($"AI model prediction failed: {aiResponse.Message}");
                }

                // 6. Save prediction result
                var prediction = new CaloriesPrediction
                {
                    UserId = request.UserId,
                    ActivityDtataId = dailyActivity.Id,
                    PredictedCalories = aiResponse.PredictedCalories,
                    PredictionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.CaloriesPredictions.CreateAsync(prediction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Calories prediction completed for user {UserId}, day {Day}. Predicted calories: {Calories}",
                    request.UserId, request.Day, aiResponse.PredictedCalories);

                // 7. Return response
                return new CaloriesPredictionResponseDto
                {
                    //UserId = request.UserId,
                   // Day = request.Day,
                    PredictedCalories = aiResponse.PredictedCalories,
                  //  UpdatedWeight = dailyActivity.WeightKg,
                    PredictionDate = prediction.PredictionDate,
                   // ActivityData = _mapper.Map<DailyActivityDataDto>(dailyActivity)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error predicting calories for user {UserId}, day {Day}", request.UserId, request.Day);
                throw;
            }
        }


        public async Task<int> ImportBulkActivityDataAsync(UserActivityJsonDto data, string userId)
        {
            try
            {
                _logger.LogInformation("Starting bulk import of activity data for user {UserId}", userId);

                // Validate data
                if (data == null || data.TotalSteps == null || data.VeryActiveMinutes == null ||
                    data.FairlyActiveMinutes == null || data.LightlyActiveMinutes == null || data.SedentaryMinutes == null ||
                    data.TotalMinutesAsleep == null || data.WeightKg == null)
                {
                    throw new ArgumentException("Invalid activity data provided");
                }

                // Ensure all dictionaries have the same keys
                var allKeys = data.TotalSteps.Keys.ToList();
                if (!data.VeryActiveMinutes.Keys.SequenceEqual(allKeys) ||
                    !data.FairlyActiveMinutes.Keys.SequenceEqual(allKeys) ||
                    !data.LightlyActiveMinutes.Keys.SequenceEqual(allKeys) ||
                    !data.SedentaryMinutes.Keys.SequenceEqual(allKeys) ||
                    !data.TotalMinutesAsleep.Keys.SequenceEqual(allKeys) ||
                    !data.WeightKg.Keys.SequenceEqual(allKeys))
                {
                    throw new ArgumentException("Activity data dictionaries must have the same keys");
                }

                // Sort keys numerically for proper day ordering
                var sortedKeys = allKeys.OrderBy(k => int.Parse(k)).ToList();
                int totalReadings = sortedKeys.Count;
                _logger.LogInformation("Processing {Count} activity records for user {UserId}", totalReadings, userId);

                int totalImported = 0;

                // Process in batches to avoid memory issues
                for (int i = 0; i < totalReadings; i += BATCH_SIZE)
                {
                    int batchSize = Math.Min(BATCH_SIZE, totalReadings - i);
                    var batch = new List<ActivityData>();

                    for (int j = 0; j < batchSize; j++)
                    {
                        int index = i + j;
                        string dayKey = sortedKeys[index];

                        batch.Add(new ActivityData
                        {
                            UserId = userId,
                            Day = int.Parse(dayKey),
                            TotalSteps = data.TotalSteps[dayKey],
                            VeryActiveMinutes = data.VeryActiveMinutes[dayKey],
                            FairlyActiveMinutes = data.FairlyActiveMinutes[dayKey],
                            LightlyActiveMinutes = data.LightlyActiveMinutes[dayKey],
                            SedentaryMinutes = data.SedentaryMinutes[dayKey],
                            TotalMinutesAsleep = data.TotalMinutesAsleep[dayKey],
                            WeightKg = data.WeightKg[dayKey],
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    await _unitOfWork.ActivityDatas.AddrangeAsync(batch);
                    await _unitOfWork.SaveChangesAsync();

                    totalImported += batch.Count;
                    _logger.LogInformation("Imported batch {BatchNumber} ({Count} activity records) for user {UserId}",
                        (i / BATCH_SIZE) + 1, batch.Count, userId);
                }

                // Clear any cache for this user to ensure fresh data

                _logger.LogInformation("Completed bulk import of {Count} activity records for user {UserId}",
                    totalImported, userId);

                return totalImported;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk import of activity data for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> ImportBulkActivityDataFromJsonFileAsync(string jsonFilePath, string userId)
        {
            try
            {
                _logger.LogInformation("Starting import of activity data from JSON file for user {UserId}", userId);

                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("JSON file not found", jsonFilePath);
                }

                // Read the file
                string jsonContent = await File.ReadAllTextAsync(jsonFilePath);

                // Deserialize
                var data = JsonSerializer.Deserialize<UserActivityJsonDto>(jsonContent);

                // Import the data
                return await ImportBulkActivityDataAsync(data, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing activity data from JSON file for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<CaloriesPredictionResponseDto>> GetUserPredictionsAsync(string userId)
        {
            try
            {
                var predictions = await _unitOfWork.CaloriesPredictions.GetUserPredictionsAsync(userId);

                return predictions.Select(p => new CaloriesPredictionResponseDto
                {
                    //UserId = p.UserId,
                   // Day = p.ActivityData.Day,
                    PredictedCalories = p.PredictedCalories,
                    //UpdatedWeight = p.ActivityData.WeightKg,
                    PredictionDate = p.PredictionDate,
                    //ActivityData = _mapper.Map<DailyActivityDataDto>(p.ActivityData)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting predictions for user {UserId}", userId);
                throw;
            }
        }
        public async Task UpdateUserWeight(string id, double weight)
        {
            try
            {
                await _unitOfWork.Users.UpdateUserWeightAsync(id, weight);
                await _unitOfWork.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating weight for user {UserId}", id);
                throw new Exception($"Error updating weight for user {id}", ex);
            }
        }
        public async Task<CaloriesPredictionResponseDto> GetLatestReadingAsync(string userId)
        {
            var data = await _unitOfWork.CaloriesPredictions.GetLatestCaloriesPredictionByUserIdAsync(userId);
            var mappeddata = _mapper.Map<CaloriesPredictionResponseDto>(data);
            return mappeddata;
        }

        public async Task<IEnumerable<CaloriesPredictionResponseDto>> GetReadingsByDateRangeAsync(string userId, DateTime start, DateTime end)
        {
            var data = await _unitOfWork.CaloriesPredictions.GetCaloriesPredictionByDateRangeAsync(userId, start, end);
            var mappeddata = _mapper.Map<IEnumerable<CaloriesPredictionResponseDto>>(data);
            return mappeddata;
        }

    }
}

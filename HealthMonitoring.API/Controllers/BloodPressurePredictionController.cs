using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodPressurePredictionController : ControllerBase
    {
        private readonly ISensorDataService _sensorDataService;
        private readonly ILogger<SensorDataController> _logger;
        private readonly IBloodPressurePredictionService _aIModelService;
        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        public BloodPressurePredictionController(ISensorDataService sensorDataService, ILogger<SensorDataController> logger
            , IBloodPressurePredictionService aIModelService, IUnitOfWork unitOfWork)
        {
            _sensorDataService = sensorDataService;
            _logger = logger;
            _aIModelService = aIModelService;
            _unitOfWork = unitOfWork;
            _response = new();
        }

        [HttpPost("send-ai")]
        [ResponseCache(Duration = 60)] // Add response caching
        public async Task<IActionResult> SendDataToAIModel([FromQuery] string userId)
        {
            try
            {
                _logger.LogInformation($"Request received for blood pressure prediction: {userId}");

                var prediction = await _aIModelService.PredictBloodPressure(userId);

                if (prediction == null || prediction.sbp == null || prediction.dbp == null)
                {
                    _logger.LogWarning($"Invalid prediction result for user {userId}");
                    return BadRequest(new { message = "Invalid prediction result from AI model" });
                }

                // Calculate average as a simple way to consolidate multiple predictions
                var avgSystolic = prediction.sbp.Average();
                var avgDiastolic = prediction.dbp.Average();

                // Determine blood pressure category
                var category = DetermineBloodPressureCategory(avgSystolic, avgDiastolic);

                _logger.LogInformation($"Blood pressure prediction completed for user {userId}: SBP={avgSystolic:F1}, DBP={avgDiastolic:F1}, Category={category}");

                // Store the prediction in the database
                await StoreBloodPressurePrediction(userId, avgSystolic, avgDiastolic, category);

                return Ok(new
                {
                    systolic = avgSystolic,
                    diastolic = avgDiastolic,
                    category = category.ToString(),
                    allReadings = new
                    {
                        systolicReadings = prediction.sbp,
                        diastolicReadings = prediction.dbp
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error predicting blood pressure for user {userId}");
                return BadRequest(new { message = ex.Message });
            }
        }



        private BloodPressureCategory DetermineBloodPressureCategory(double systolic, double diastolic)
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

        private async Task StoreBloodPressurePrediction(string userId, double systolic, double diastolic, BloodPressureCategory category)
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
    }
}

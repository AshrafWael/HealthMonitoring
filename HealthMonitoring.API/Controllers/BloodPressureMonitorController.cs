using Azure;
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.Services;
using HealthMonitoring.DAL.Data.Models.AIModels;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly ISensorDataService _sensorDataService;
        private readonly ILogger<SensorDataController> _logger;
        private readonly IAIModelService _aIModelService;
        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        public SensorDataController(ISensorDataService sensorDataService, ILogger<SensorDataController> logger
            ,IAIModelService aIModelService,IUnitOfWork unitOfWork)
        {
            _sensorDataService = sensorDataService;
            _logger = logger;
           _aIModelService = aIModelService;
            _unitOfWork = unitOfWork;
            _response = new();

        }


        /*
        [HttpPost]
        //  [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AddDataSet([FromBody] DataPointDto dataPointDto, string UserId)
        {
            try
            {
                if (dataPointDto == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.Result = dataPointDto;
                    return BadRequest(_response);
                }
                await _sensorDataService.AddDatasetToUser(dataPointDto);
                _response.IsSuccess = true;
                _response.Result = dataPointDto;
                _response.StatusCode = HttpStatusCode.Created;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;

        }

        */

        [HttpPost("import")]
        public async Task<IActionResult> ImportSensorData([FromBody] DataPointDto data, [FromQuery] string userId)
        {
            try
            {
                if (data == null || data.PPG == null || data.ABP == null || data.ECG == null)
                {
                    return BadRequest("Invalid sensor data format");
                }

                // Check if data arrays have the same length
                if (data.PPG.Count != data.ABP.Count || data.PPG.Count != data.ECG.Count)
                {
                    return BadRequest("Sensor data arrays must have the same length");
                }

                int totalImported = await _sensorDataService.ImportBulkSensorDataAsync(data, userId);

                // Also cache the dataset for quick access
                string cacheKey = $"user_{userId}_dataset";
                await _sensorDataService.CacheDatasetAsync(data, cacheKey);

                return Ok(new
                {
                    message = $"Successfully imported {totalImported} sensor readings",
                    totalReadings = totalImported
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing sensor data for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while importing sensor data" });
            }
        }

        [HttpPost("import/file")]
        public async Task<IActionResult> ImportSensorDataFile(IFormFile file, [FromQuery] string userId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                // Save the file temporarily
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Process the file
                int totalImported = await _sensorDataService.ImportBulkSensorDataFromJsonFileAsync(filePath, userId);

                // Clean up
               // System.IO.File.Delete(filePath);

                return Ok(new
                {
                    message = $"Successfully imported {totalImported} sensor readings from file",
                    totalReadings = totalImported
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing sensor data file for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while importing sensor data file" });
            }
        }

        [HttpGet("cached/{userId}")]
        public async Task<IActionResult> GetCachedDataset(string userId)
        {
            try
            {
                string cacheKey = $"user_{userId}_dataset";
                var cachedData = await _sensorDataService.GetCachedDatasetAsync(cacheKey);

                if (cachedData == null)
                {
                    return NotFound(new { message = "No cached dataset found for this user" });
                }

                return Ok(new
                {
                    message = "Retrieved cached dataset",
                    datasetSize = new
                    {
                        ppgCount = cachedData.PPG.Count,
                        abpCount = cachedData.ABP.Count,
                        ecgCount = cachedData.ECG.Count
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached dataset for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while retrieving cached dataset" });
            }
        }
        [HttpGet("Get-ById")]
        public async Task<IActionResult> GetDatasetByUserId(string userid)
        {
        Console.WriteLine("Received request");
            var data = await _sensorDataService.GetDataSetByUser(userid);
            //   return Ok(data);

            return Ok(new
            {
                message = "Retrieved  dataset",
                datasetSize = new DataPointDto
                {
                    PPG = data.FirstOrDefault().PPG,
                    ABP = data.FirstOrDefault().ABP,
                    ECG = data.FirstOrDefault().ECG
                }
            });

        }
        /*
        [HttpPost("send-ai")]
        public async Task<IActionResult> SendDataToAIModel([FromQuery] string userId)
        {
            try
            {
                var prediction = await _aIModelService.PredictBloodPressure(userId);
                return Ok(new
                {
                    message = "Blood pressure prediction received successfully!",
                    spb = prediction.SystolicPressure,
                    dbp = prediction.DiastolicPressure
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        */

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


        /*
        [HttpPost("send-ai")]
      //  [ResponseCache(Duration = 60)]
        public async Task<IActionResult> SendDataToAIModel([FromQuery] string userId)
        {
            try
            {
                // Validate userId
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("SendDataToAIModel called with null or empty userId");
                    return BadRequest(new { message = "UserId is required" });
                }

                _logger.LogInformation($"Request received for blood pressure prediction: {userId}");

                // Check if service is properly injected
                if (_aIModelService == null)
                {
                    _logger.LogError("AIModelService is null - check DI configuration");
                    return StatusCode(500, new { message = "Server configuration error" });
                }

                // Get prediction and add null check
                var prediction = await _aIModelService.PredictBloodPressure(userId);

                if (prediction == null)
                {
                    _logger.LogWarning($"Null prediction result returned for user {userId}");
                    return BadRequest(new { message = "Blood pressure prediction failed" });
                }

                // Check prediction data
                if (prediction.sbp == null || prediction.dbp == null ||
                    !prediction.sbp.Any() || !prediction.dbp.Any())
                {
                    _logger.LogWarning($"Prediction contains null or empty data for user {userId}");
                    return BadRequest(new { message = "Invalid prediction data" });
                }

                // Safe to calculate average now
                var avgSystolic = prediction.sbp.Average();
                var avgDiastolic = prediction.dbp.Average();

                _logger.LogInformation($"Blood pressure prediction completed for user {userId}");

                return Ok(new
                {
                    systolic = avgSystolic,
                    diastolic = avgDiastolic,
                    systolicReadings = prediction.sbp,
                    diastolicReadings = prediction.dbp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error predicting blood pressure for user {userId}");
                return StatusCode(500, new { message = "An error occurred while processing the request" });
            }
        }
        */
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

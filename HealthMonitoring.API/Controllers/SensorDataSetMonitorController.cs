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
        private readonly IBloodPressurePredictionService _aIModelService;
        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        public SensorDataController(ISensorDataService sensorDataService, ILogger<SensorDataController> logger
            ,IBloodPressurePredictionService aIModelService,IUnitOfWork unitOfWork)
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
        public async Task<IActionResult> ImportSensorData([FromBody] DataSetDto data, [FromQuery] string userId)
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
            var data = await _sensorDataService.GetDataSetByUser(userid);
            //   return Ok(data);
            if (data == null)
            {
                return NotFound("No data found for this user.");
            }
            return Ok(new
            {
                    PPG = data.FirstOrDefault().PPG.ToList(),
                    ABP = data.FirstOrDefault().ABP.ToList(),
                    ECG = data.FirstOrDefault().ECG.ToList()        
            });


        }
  


    }
}

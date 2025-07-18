﻿using System.Net;
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.Services;
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
        public async Task<ActionResult<APIResponse>> SendDataToAIModel([FromQuery] string userId,int batchsize)
        {
            try
            {
              
                _logger.LogInformation($"Request received for blood pressure prediction: {userId}");

                var prediction = await _aIModelService.PredictBloodPressure(userId, batchsize);

                if (prediction == null || prediction.sbp == null || prediction.dbp == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                // Calculate average as a simple way to consolidate multiple predictions
                var avgSystolic = prediction.sbp.Average();
                var avgDiastolic = prediction.dbp.Average()+10;
                // Determine blood pressure category
                var category = _aIModelService.DetermineBloodPressureCategory(avgSystolic, avgDiastolic);
                // Store the prediction in the database
                await _aIModelService.StoreBloodPressurePrediction(userId, avgSystolic, avgDiastolic, category);
                _response.StatusCode = HttpStatusCode.OK;
                 _response.IsSuccess = true;
                _response.Result =  new 
                    {
                    systolic = avgSystolic,
                    diastolic = avgDiastolic,
                    category = category.ToString(),
                };
                
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
        [HttpGet("GetAllByUserId/{userId}")]
        public async Task<ActionResult<APIResponse>> GetRecentReadings(string userId)
        {
            try
            {
                var result = await _aIModelService.GetRecentReadingsAsync(userId);
                if (result == null || !result.Any())
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Heart rate data not found.");
                    return NotFound(_response);
                }

                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add(ex.Message);
                return StatusCode((int)_response.StatusCode, _response);
            }
        }
        [HttpGet("GetLatestByUserId/{userId}")]
        public async Task<ActionResult<APIResponse>> GetLatestReading(string userId)
        {
            try
            {
                var result = await _aIModelService.GetLatestReadingAsync(userId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Heart rate data not found.");
                    return NotFound(_response);
                }

                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add(ex.Message);
                return StatusCode((int)_response.StatusCode, _response);
            }
        }
        [HttpGet("GetRangeByUserId/{userId}")]
        public async Task<ActionResult<APIResponse>> GetReadingsByDateRange(string userId, DateTime startdate, DateTime enddata)
        {
            try
            {
                var result = await _aIModelService.GetReadingsByDateRangeAsync(userId, startdate, enddata);
                if (result == null || !result.Any())
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Heart rate data not found.");
                    return NotFound(_response);
                }

                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add(ex.Message);
                return StatusCode((int)_response.StatusCode, _response);
            }
        }

    }
}

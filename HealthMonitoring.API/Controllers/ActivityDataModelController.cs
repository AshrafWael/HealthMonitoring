using Azure;
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.Services;
using HealthMonitoring.DAL.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityDataModelController : ControllerBase
    {
        private readonly ICaloriesPredictionService _caloriesPredictionService;
        protected APIResponse _response;

        public ActivityDataModelController(ICaloriesPredictionService caloriesPredictionService)
        {
           _caloriesPredictionService = caloriesPredictionService;
            _response = new();
        }
        [HttpPost("importActivity/file")]
        public async Task<IActionResult> ImportActivityFile(IFormFile file, [FromQuery] string userId)
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
                int totalImported = await _caloriesPredictionService.ImportBulkActivityDataFromJsonFileAsync(filePath, userId);

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
                return StatusCode(500, new { error = "An error occurred while importing sensor data file" });
            }
        }
        [HttpPost("predict")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> PredictCalories([FromBody] CaloriesPredictionRequestDto request)
        {
            try
            {
                if (request.Day < 0 || request.Day > 30)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors.Add("Day must be between 0 and 30");
                    return BadRequest(_response);
                }

                var result = await _caloriesPredictionService.PredictCaloriesForUserDayAsync(request);
                _response.Result = result;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(
                    new
                    {
                        Success = true,
                        Message = "Calories prediction successful.",
                        Data = result
                    });
                // return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add(ex.Message);
                return StatusCode((int)_response.StatusCode, _response);
            }
        }
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetActivityDataByUserId(string userId)
        {
            try
            {
                var predictions = await _caloriesPredictionService.GetUserPredictionsAsync(userId);
                if (predictions == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Activity data not found.");
                    return NotFound(_response);
                }

                _response.Result = predictions;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
                //return Ok(new
                //{
                //    Success = true,
                //    Message = "Predictions retrieved successfully",
                //    Data = predictions,
                //  //  Count = predictions.Count()
                //});

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
                var result = await _caloriesPredictionService.GetLatestReadingAsync(userId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("calories data not found.");
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
                var result = await _caloriesPredictionService.GetReadingsByDateRangeAsync(userId, startdate, enddata);
                if (result == null || !result.Any())
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("calories data not found.");
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
        [HttpPut("{id}/weight")]
        public async Task<IActionResult> UpdateUserWeight(string id, [FromBody] double weight)
        {
            try
            {
                await _caloriesPredictionService.UpdateUserWeight(id, weight);

                return Ok(new
                {
                    Success = true,
                    Message = "User weight updated successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Internal server error" });
            }
        }

    }
}

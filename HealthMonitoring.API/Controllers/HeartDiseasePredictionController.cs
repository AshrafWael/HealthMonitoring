using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.IServices;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeartDiseasePredictionController : ControllerBase
    {
        private readonly ILogger<SensorDataController> _logger;
        private readonly IHeartDiseaseService _aIModelService;
        protected APIResponse _response;
        public HeartDiseasePredictionController(ILogger<SensorDataController> logger
            , IHeartDiseaseService aIModelService)
        {
            _logger = logger;
            _aIModelService = aIModelService;
            _response = new();
        }

        [HttpPost("send-ai")]
        [ResponseCache(Duration = 60)] // Add response caching
        public async Task<ActionResult<APIResponse>> SendDataToAIModel([FromQuery] string userId, int batchsize)
        {
            try
            {

                _logger.LogInformation($"Request received for blood pressure prediction: {userId}");

                var prediction = await _aIModelService.PredictHeartDisease(userId, batchsize);

                //if (prediction == null || prediction.Disease == null)
                if (prediction == null )

                {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var heartdisease = prediction.Prediction;
                await _aIModelService.StoreHeartDisease(userId, heartdisease);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new
                {
                    prediction = prediction
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
        public async Task<ActionResult<APIResponse>> GetUserHeartDiseaseData(string userId)
        {
            try
            {
                var result = await _aIModelService.GetUserHeartDiseaseDataAsync(userId);
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
        public async Task<ActionResult<APIResponse>> GetLatestHeartDisease(string userId)
        {
            try
            {
                var result = await _aIModelService.GetLatestHeartDiseaseAsync(userId);
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
        public async Task<ActionResult<APIResponse>> GetHeartDiseaseByDateRange(string userId, DateTime startdate, DateTime enddata)
        {
            try
            {
                var result = await _aIModelService.GetHeartDiseaseByDateRangeAsync(userId, startdate, enddata);
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

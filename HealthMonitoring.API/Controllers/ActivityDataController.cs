using Azure;
using System.Net;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.DAL.Data.Models;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityDataController : ControllerBase
    {
        private readonly IActivityDataServices _activityDataServices;
        protected APIResponse _response;

        public ActivityDataController(IActivityDataServices activityDataServices)
        {
            _activityDataServices = activityDataServices;
            _response = new();
        }
        
        [HttpGet("GetAllActivits")]
        public   async Task<IActionResult >  GetAllActivits() 
        {
            var   activitys = await _activityDataServices.GetAll();
            return Ok(activitys);
        }
        [HttpGet("activity-data/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetActivityDataByUserId(string userId)
        {
            try
            {
                var result = await _activityDataServices.GetByUserId(userId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Activity data not found.");
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AddActivityData([FromBody] ActivityDataCreateDto createdActivity)
        {
            try
            {   
                if (createdActivity == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.Result = createdActivity;
                    return BadRequest(_response);
                }
                await _activityDataServices.Add(createdActivity);
                _response.IsSuccess = true;
                _response.Result = createdActivity;
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
        [HttpPut("activity-data/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateActivityData([FromBody] ActivityDataUpdateDto activityUpdateDto,int id)
        {
            try
            {
                if (activityUpdateDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors.Add("Update data is null.");
                    return BadRequest(_response);
                }

                var updated = await _activityDataServices.Update(activityUpdateDto,id);
                _response.Result = updated;
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
        [HttpDelete("activity-data/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteActivityData(int Id)
        {
            try
            {
                await _activityDataServices.Delete(Id);
                _response.Result = "Activity data deleted successfully.";
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (KeyNotFoundException)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors.Add("Activity data not found.");
                return NotFound(_response);
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

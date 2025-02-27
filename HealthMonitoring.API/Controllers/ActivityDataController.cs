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
        [HttpPost]
        //  [Authorize(Roles = "admin")]
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
    }
}

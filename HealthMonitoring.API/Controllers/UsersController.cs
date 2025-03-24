using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.Dtos.AccountUserDtos;
using HealthMonitoring.DAL.IRepository;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthMonitoring.DAL.UnitOfWork;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Services;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        protected APIResponse _response;
        public UsersController(IAuthServices authServices)
        {
           
            _response = new();
            _authServices = authServices;
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginresponse = await _authServices.Login(loginRequestDto);
            if (loginresponse.User == null || string.IsNullOrEmpty(loginresponse.Token))
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("UserName Or Password Is Incorrect");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = loginresponse;
            return Ok(_response);

        }
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        
        {
            bool usernameuniqu =await _authServices.IsUniqueUser(registerRequestDto.UserName);
            if (!usernameuniqu)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("UserNmae Is Already Exist");
                return BadRequest(_response);
            }
            var user = await _authServices.Register(registerRequestDto);
            if (user == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Error While Registration");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = user;
            return Ok(_response);
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateUser")]
        public async Task<ActionResult<APIResponse>> UpdateUser(string id, ApplicationUserUpdateDto userUpdateDto)
        {
            try
            {
                if (userUpdateDto == null || id != userUpdateDto.ID)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;

                    return BadRequest(_response);
                }
               await _authServices.UpdateUser(userUpdateDto, id);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _response.Result=userUpdateDto;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;
        }




    


    }
}

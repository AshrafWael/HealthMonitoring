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
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos.AccountUserDtos;
using HealthMonitoring.BLL.Dtos.MailingDto;
using Microsoft.AspNetCore.Identity;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IMailingService _mailingService;
        protected APIResponse _response;
        public UsersController(IAuthServices authServices,IMailingService mailingService)
        {
           
            _response = new();
            _authServices = authServices;
            _mailingService = mailingService;
        }
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            bool usernameuniqu = await _authServices.IsUniqueUser(registerRequestDto.UserName);
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
                _response.Errors.Add("UserName Or Password Is Incorrect Or Confirm Your Email");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = loginresponse;
            return Ok(_response);

        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authServices.Logout();
            return Ok(result);
        }
        [HttpPut("Update{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordData)
        {
            var changePassword = await _authServices.ResetPassword(resetPasswordData);

            //not best practice
            if (changePassword == "Password Changed Correctly")
            {
                return Ok(changePassword);
            }

            return BadRequest(changePassword);
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _authServices.FindByEmail(email);
            if (user == null)
                return BadRequest("User not found.");
           
            await _authServices.SendPasswordResetEmailAsync(user);
            return Ok("Password reset email sent.");
        }
      
        [HttpPost("Send-Email")]
        public async Task<IActionResult> SendEmail([FromForm] MailRequstDto mailRequst)
        { 
        await _mailingService.SendEmailAsync(mailRequst.ToEmail,mailRequst.Subject,mailRequst.Body,mailRequst.attachments);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = mailRequst;
            return Ok(_response);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
             if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation link");
            var result =  await _authServices.ConfirmEmail(userId, token);
            if (result == false)
                return BadRequest("Invalid email confirmation link");
            if (result == true)
                return Ok("Email confirmed successfully!");
            return BadRequest("Failed to confirm email");
        }
        [HttpPost("ResetForgotPassword")]
        public async Task<IActionResult> ResetForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.Token) || string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Invalid reset request.");
            var success = await _authServices.ResetForgotPasswordAsync(dto);
            if (!success)
                return BadRequest("Failed to reset password. Token may be invalid or expired.");
            return Ok("Password reset successful.");
        }

    }
}

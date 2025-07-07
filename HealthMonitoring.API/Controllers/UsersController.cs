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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;

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
            bool usernameuniqu = await _authServices.IsUniqueUser(registerRequestDto.UserName,registerRequestDto.Email);
            if (!usernameuniqu)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("UserNmae or email Is Already Exist");
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
                _response.Errors.Add("UserName Or Password Is Incorrect ");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = loginresponse;
            return Ok(_response);

        }
        [HttpPost("logout")]
       // [Authorize] // Ensure user is authenticated
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequest)
        {
            try
            {
                // Get user ID from claims if not provided in request
                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                     User.FindFirst(ClaimTypes.Name)?.Value;

                // Use provided userId or fall back to token userId
                var userId = !string.IsNullOrEmpty(logoutRequest?.UserId) ?
                            logoutRequest.UserId : userIdFromToken;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new LogoutResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID is required",
                        Errors = { "No user ID provided in request or token" },
                        LogoutTime = DateTime.UtcNow
                    });
                }

                // Get token from Authorization header if not provided in body
                var token = logoutRequest?.Token;
                if (string.IsNullOrEmpty(token))
                {
                    token = Request.Headers["Authorization"]
                        .FirstOrDefault()?.Split(" ").LastOrDefault();
                }

                // Perform logout
                var result = await _authServices.LogoutAsync(userId, token);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new LogoutResponseDto
                {
                    IsSuccess = false,
                    Message = "Internal server error during logout",
                    Errors = { "An unexpected error occurred" },
                    LogoutTime = DateTime.UtcNow
                });
            }
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
        [HttpGet("userdata/by-id/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetUserDataById(string userId)
        {
            try
            {
                var result = await _authServices.GetUserDataById(userId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("User not found.");
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
        [HttpGet("ResetPasswordpage")]
        public IActionResult ResetPasswordPage()
        {
            var htmlContent = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reset Password</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }
        
        .reset-container {
            background: white;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 450px;
            animation: slideIn 0.3s ease-out;
        }
        
        @keyframes slideIn {
            from {
                opacity: 0;
                transform: translateY(-20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .reset-container h2 {
            text-align: center;
            color: #333;
            margin-bottom: 30px;
            font-weight: 600;
        }
        
        .form-group {
            margin-bottom: 25px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #555;
            font-weight: 500;
        }
        
        .form-group input {
            width: 100%;
            padding: 12px 16px;
            border: 2px solid #e1e5e9;
            border-radius: 8px;
            font-size: 16px;
            transition: border-color 0.3s ease;
        }
        
        .form-group input:focus {
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }
        
        .password-requirements {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 6px;
            margin-top: 10px;
            font-size: 14px;
            color: #666;
        }
        
        .password-requirements ul {
            margin: 8px 0 0 20px;
        }
        
        .reset-btn {
            width: 100%;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 14px;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }
        
        .reset-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(102, 126, 234, 0.3);
        }
        
        .reset-btn:disabled {
            background: #ccc;
            cursor: not-allowed;
            transform: none;
            box-shadow: none;
        }
        
        .loading {
            display: none;
            text-align: center;
            margin-top: 15px;
            color: #666;
        }
        
        .loading::after {
            content: '';
            display: inline-block;
            width: 20px;
            height: 20px;
            margin-left: 10px;
            border: 2px solid #ccc;
            border-top: 2px solid #667eea;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        
        .message {
            margin-bottom: 20px;
            padding: 12px;
            border-radius: 6px;
            text-align: center;
            display: none;
        }
        
        .message.success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .message.error {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        .back-link {
            text-align: center;
            margin-top: 20px;
        }
        
        .back-link a {
            color: #667eea;
            text-decoration: none;
            font-weight: 500;
        }
        
        .back-link a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <div class=""reset-container"">
        <h2>Reset Your Password</h2>
        
        <div id=""message"" class=""message""></div>
        
        <form id=""resetForm"">
            <div class=""form-group"">
                <label for=""newPassword"">New Password</label>
                <input type=""password"" id=""newPassword"" name=""newPassword"" required>
            </div>
            
            <div class=""form-group"">
                <label for=""confirmPassword"">Confirm New Password</label>
                <input type=""password"" id=""confirmPassword"" name=""confirmPassword"" required>
            </div>
            
            <div class=""password-requirements"">
                <strong>Password Requirements:</strong>
                <ul>
                    <li>At least 8 characters long</li>
                    <li>Contains at least one uppercase letter</li>
                    <li>Contains at least one lowercase letter</li>
                    <li>Contains at least one number</li>
                    <li>Contains at least one special character</li>
                </ul>
            </div>
            
            <button type=""submit"" class=""reset-btn"">Reset Password</button>
        </form>
        
        <div id=""loading"" class=""loading"">Processing your request...</div>   
    </div>

    <script>
        // Get URL parameters
        function getUrlParams() {
            const urlParams = new URLSearchParams(window.location.search);
            return {
                userId: urlParams.get('userId'),
                token: urlParams.get('token')
            };
        }

        // Show message
        function showMessage(text, type) {
            const messageEl = document.getElementById('message');
            messageEl.textContent = text;
            messageEl.className = `message ${type}`;
            messageEl.style.display = 'block';
        }

        // Hide message
        function hideMessage() {
            document.getElementById('message').style.display = 'none';
        }

        // Validate password
        function validatePassword(password) {
            const minLength = 8;
            const hasUpperCase = /[A-Z]/.test(password);
            const hasLowerCase = /[a-z]/.test(password);
            const hasNumbers = /\d/.test(password);
            const hasSpecialChar = /[!@#$%^&*(),.?\"":{}|<>]/.test(password);

            return password.length >= minLength && hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChar;
        }

        // Handle form submission
        document.getElementById('resetForm').addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const newPassword = document.getElementById('newPassword').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const { userId, token } = getUrlParams();
            
            hideMessage();
            
            // Validation
            if (!userId || !token) {
                showMessage('Invalid reset link. Please request a new password reset.', 'error');
                return;
            }
            
            if (newPassword !== confirmPassword) {
                showMessage('Passwords do not match.', 'error');
                return;
            }
            
            if (!validatePassword(newPassword)) {
                showMessage('Password does not meet requirements.', 'error');
                return;
            }
            
            // Show loading
            document.querySelector('.reset-btn').disabled = true;
            document.getElementById('loading').style.display = 'block';
            
            try {
                const response = await fetch('/api/Users/ResetForgotPassword', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        userId: userId,
                        token: decodeURIComponent(token),
                        newPassword: newPassword
                    })
                });
                
                if (response.ok) {
                    showMessage('Password reset successful! You can now login with your new password.', 'success');
                    document.getElementById('resetForm').style.display = 'none';
                } else {
                    const errorText = await response.text();
                    showMessage(errorText || 'Failed to reset password. The link may be expired or invalid.', 'error');
                }
            } catch (error) {
                showMessage('An error occurred. Please try again.', 'error');
            } finally {
                document.querySelector('.reset-btn').disabled = false;
                document.getElementById('loading').style.display = 'none';
            }
        });

        // Check if parameters exist on page load
        window.addEventListener('DOMContentLoaded', function() {
            const { userId, token } = getUrlParams();
            if (!userId || !token) {
                showMessage('Invalid reset link. Please request a new password reset.', 'error');
                document.getElementById('resetForm').style.display = 'none';
            }
        });
    </script>
</body>
</html>";

            return Content(htmlContent, "text/html");
        }

      

    }
}

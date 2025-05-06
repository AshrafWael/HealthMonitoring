using HealthMonitoring.BLL.Dtos.SMSDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.Services;
using HealthMonitoring.BLL.StaticData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly ISMSService _Sms;
        public SMSController( ISMSService sMS)
        {
            _Sms = sMS;
        }
        [HttpPost("SendSMS")]
        public IActionResult SendSMS(SendSMSDto dto)
        {
            try
            {
                var result = _Sms.SendMessage(dto.PhoneNumber, dto.Body);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> Send([FromBody] SmsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _Sms.SendSmsAsync(request.PhoneNumber, request.Message);
                return Ok(new { success = true, message = "SMS sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}


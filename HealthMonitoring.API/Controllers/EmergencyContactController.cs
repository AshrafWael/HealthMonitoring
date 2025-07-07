using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;
using HealthMonitoring.BLL.IServices;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using HealthMonitoring.BLL.ApiResponse;
using HealthMonitoring.BLL.Services;
using System.Security.Claims;
using Vonage.Users;
using HealthMonitoring.BLL.APIRequst;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyContactController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IEmergencyContactService _service;
        private readonly APIResponse _response;

        public EmergencyContactController(IAuthServices authServices, IEmergencyContactService service)
        {
          _authServices = authServices;
            _service = service;
            _response = new APIResponse();
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateEmergencyContact(string Userid ,CreateEmergencyContactDto createDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = errors;
                    return BadRequest(_response);
                }

                var result = await _service.CreateEmergencyContactAsync(Userid, createDto);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = result;
                return CreatedAtAction(nameof(GetContactById), new { id = result.ContactId }, _response);
            }

            catch (InvalidOperationException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

        }
        // Get user with all emergency contacts
        [HttpGet("{userId}/emergency-contacts")]
        public async Task<ActionResult<UserDto>> GetUserWithEmergencyContacts(string userId)
        {
            try
            {
                 var user = await _authServices.GetUserWithEmergencyContactsAsync(userId);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("failed");
                    return NotFound(_response);
                }

                _response.Result = user;
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

        // Get user by email
        [HttpGet("{email}/get-emergency-contacts")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
            var user = await _authServices.GetUserByEmailAsync(email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("failed");
                    return NotFound(_response);
                }

                _response.Result = user;
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

        //// Get all emergency contacts for a user by user ID
        //[HttpGet("user/{userId}")]
        //public async Task<ActionResult<IEnumerable<EmergencyContactDto>>> GetContactsByUserId(string userId)
        //{
        //    var contacts = await _service.GetContactsByUserIdAsync(userId);
        //    return Ok(contacts);
        //}

        //// Get all emergency contacts for a user by email
        //[HttpGet("user/email/{email}")]
        //public async Task<ActionResult<IEnumerable<EmergencyContactDto>>> GetContactsByUserEmail(string email)
        //{
        //    var contacts = await _service.GetContactsByUserEmailAsync(email);
        //    return Ok(contacts);
        //}

        // Get all users connected to an emergency contact
        [HttpGet("{contactId}/get-users")]
        public async Task<ActionResult<IEnumerable<ConectedUserDto>>> GetUsersByContactId(int contactId)
        {
            try
            {
            var result = await _service.GetUsersByContactIdAsync(contactId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("failed.");
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

        // Get emergency contact with all connected users
        [HttpGet("{id}/emergency-contact-with-users")]
        public async Task<ActionResult<EmergencyContactDto>> GetContactById(int id)
        {


            try
            {
            var contact = await _service.GetContactWithUsersAsync(id);
                if (contact == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("failed to get contact.");
                    return NotFound(_response);
                }

                _response.Result = contact;
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

        // Connect user to emergency contact by email
        [HttpPost("connect")]
        public async Task<ActionResult> ConnectUserToContact([FromBody] ConnectContactDto connectDto)
        {
            try
            {
                 var result = await _service.ConnectUserToContactAsync(connectDto.UserEmail, connectDto.ContactId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("CAN NOT CONECCT USER.");
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

        // Disconnect user from emergency contact
        [HttpDelete("disconnect/{userId}/{contactId}")]
        public async Task<ActionResult> DisconnectUserFromContact(string userId, int contactId)
        {
            try
            {
                var result = await _service.DisconnectUserFromContactAsync(userId, contactId);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("CA NOT DISCONNECT USER.");
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

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
            var user = await _authServices.GetUserWithEmergencyContactsAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // Get user by email
        [HttpGet("{email}/get-emergency-contacts")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            var user = await _authServices.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
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
            var users = await _service.GetUsersByContactIdAsync(contactId);
            return Ok(users);
        }

        // Get emergency contact with all connected users
        [HttpGet("{id}/emergency-contact-with-users")]
        public async Task<ActionResult<EmergencyContactDto>> GetContactById(int id)
        {
            var contact = await _service.GetContactWithUsersAsync(id);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        // Connect user to emergency contact by email
        [HttpPost("connect")]
        public async Task<ActionResult> ConnectUserToContact([FromBody] ConnectContactDto connectDto)
        {
            var result = await _service.ConnectUserToContactAsync(connectDto.UserEmail, connectDto.ContactId);

            if (!result)
                return BadRequest("Unable to connect user to emergency contact. User or contact not found.");

            return Ok("User successfully connected to emergency contact.");
        }

        // Disconnect user from emergency contact
        [HttpDelete("disconnect/{userId}/{contactId}")]
        public async Task<ActionResult> DisconnectUserFromContact(string userId, int contactId)
        {
            var result = await _service.DisconnectUserFromContactAsync(userId, contactId);

            if (!result)
                return BadRequest("Unable to disconnect user from emergency contact.");

            return Ok("User successfully disconnected from emergency contact.");
        }
   
        
    }
}

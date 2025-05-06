using HealthMonitoring.API.ApiResponse;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;
using HealthMonitoring.BLL.IServices;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HealthMonitoring.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyContactController : ControllerBase
    {
        private readonly IEmergencyContactService _service;
        private readonly APIResponse _response;

        public EmergencyContactController(IEmergencyContactService service)
        {
            _service = service;
            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAll()
        {
            try
            {
                var contacts = await _service.GetAllAsync();
                _response.Result = contacts;
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetById(int id)
        {
            try
            {
                var contact = await _service.GetByIdAsync(id);
                if (contact == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Contact not found.");
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] EmergencyContactCreateDto dto)
        {
            try
            {
                await _service.AddAsync(dto);
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = "Contact created successfully.";
                return StatusCode((int)_response.StatusCode, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add(ex.Message);
                return BadRequest(_response);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] EmergencyContactUpdateDto dto)
        {
            try
            {
                if (id != dto.ContactId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors.Add("ID mismatch.");
                    return BadRequest(_response);
                }

                await _service.UpdateAsync(dto);
                _response.StatusCode = HttpStatusCode.NoContent;
                return StatusCode((int)_response.StatusCode, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add(ex.Message);
                return StatusCode((int)_response.StatusCode, _response);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Delete(int id)
        {
            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("Contact not found.");
                    return NotFound(_response);
                }

                await _service.DeleteAsync(id);
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok( _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Errors.Add(ex.Message);
                return Ok( _response);
            }
        }
    }
    }

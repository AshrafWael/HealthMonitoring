using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.APIRequst;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.UnitOfWork;
using Vonage.Users;

namespace HealthMonitoring.BLL.Services
{
    public class EmergencyContactService :IEmergencyContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmergencyContactService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<EmergencyContactDto>> GetAllAsync()
        {
            var contacts = await _unitOfWork.EmergancyContacts.GetAllAsync();
            return _mapper.Map<IEnumerable<EmergencyContactDto>>(contacts);
        }
        public async Task<EmergencyContactDto> GetByIdAsync(int id)
        {
            var contact = await _unitOfWork.EmergancyContacts.GetByIdAsync(id);
            return _mapper.Map<EmergencyContactDto>(contact);
        }
        public async Task AddAsync(CreateEmergencyContactDto dto)
        {
            var contact = _mapper.Map<EmergencyContact>(dto);
            await _unitOfWork.EmergancyContacts.CreateAsync(contact);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(EmergencyContactUpdateDto dto)
        {
            var contact = await _unitOfWork.EmergancyContacts.GetByIdAsync(dto.ContactId);
            if (contact != null)
            {
                _mapper.Map(dto, contact);
               await _unitOfWork.EmergancyContacts.UpdateAsync(contact);
              await  _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(int id)
        {
            var contact = await _unitOfWork.EmergancyContacts.GetByIdAsync(id);
            if (contact != null)
            {
               await _unitOfWork.EmergancyContacts.RemoveAsync(contact);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task AddEmergencyContactAsync(string userId, CreateEmergencyContactDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var contact = _mapper.Map<EmergencyContact>(dto);

            if (contact == null)
                throw new Exception("Mapping from DTO to EmergencyContact failed.");

            await _unitOfWork.EmergancyContacts.AddContactToUserAsync(userId, contact);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<EmergencyContactDto>> GetUserContactsAsync(string userId)
        {
            var contacts = await _unitOfWork.EmergancyContacts.GetContactsByUserIdAsync(userId);
            return _mapper.Map<List<EmergencyContactDto>>(contacts);
        }
        public async Task<List<UserDto>> GetUsersByContactEmailAsync(string contactEmail)
        {
            var users = await _unitOfWork.EmergancyContacts.GetUsersByContactEmailAsync(contactEmail);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<EmergencyContactResponseDto> CreateEmergencyContactAsync(CreateEmergencyContactDto createDto)
        {
            try
            {

                var emergencyContact = _mapper.Map<EmergencyContact>(createDto);

                if (!string.IsNullOrEmpty(createDto.UserId))
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(createDto.UserId);
                    if (user != null)
                    {
                        emergencyContact.ApplicationUsers.Add(user);
                    }
                }

                var createdContact =   _unitOfWork.EmergancyContacts.CreateAsync(emergencyContact);

                return _mapper.Map<EmergencyContactResponseDto>(createdContact);
            }
            catch
            {
                throw;
            }
        }
        public async Task<EmergencyContactResponseDto> CreateEmergencyContactAsync(string userid,CreateEmergencyContactDto createDto)
        {
            try
            {


             var contact =   _mapper.Map<EmergencyContact>(createDto);
                // Create the emergency contact
                var emergencyContact = await _unitOfWork.EmergancyContacts
                    .CreateEmergencyContactForUserAsync(userid, contact);
                return _mapper.Map<EmergencyContactResponseDto>(emergencyContact);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<EmergencyContactDto>> GetContactsByUserIdAsync(string userId)
        {
            var contacts = await _unitOfWork.EmergancyContacts.GetContactsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<EmergencyContactDto>>(contacts);
        }

        public async Task<IEnumerable<EmergencyContactDto>> GetContactsByUserEmailAsync(string email)
        {
            var contacts = await _unitOfWork.EmergancyContacts.GetContactsByUserEmailAsync(email);
            return _mapper.Map<IEnumerable<EmergencyContactDto>>(contacts);
        }

        public async Task<IEnumerable<ConectedUserDto>> GetUsersByContactIdAsync(int contactId)
        {
            var users = await _unitOfWork.Users.GetUsersByEmergencyContactIdAsync(contactId);
            return _mapper.Map<IEnumerable<ConectedUserDto>>(users);
        }

        public async Task<bool> ConnectUserToContactAsync(string userEmail, int contactId)
        {
            try
            {

                var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
                var contact = await _unitOfWork.EmergancyContacts.GetContactWithUsersAsync(contactId);

                if (user == null || contact == null)
                    return false;

                if (!contact.ApplicationUsers.Any(u => u.Id == user.Id))
                {
                    contact.ApplicationUsers.Add(user);
                    await _unitOfWork.EmergancyContacts.UpdateAsync(contact);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DisconnectUserFromContactAsync(string userId, int contactId)
        {
            try
            {

                var contact = await _unitOfWork.EmergancyContacts.GetContactWithUsersAsync(contactId);
                if (contact == null) return false;

                var userToRemove = contact.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
                if (userToRemove != null)
                {
                    contact.ApplicationUsers.Remove(userToRemove);
                    await _unitOfWork.EmergancyContacts.UpdateAsync(contact);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EmergencyContactWithUsersDto> GetContactWithUsersAsync(int contactId)
        {
            var contact = await _unitOfWork.EmergancyContacts.GetContactWithUsersAsync(contactId);
            return _mapper.Map<EmergencyContactWithUsersDto>(contact);
        }

    }
}

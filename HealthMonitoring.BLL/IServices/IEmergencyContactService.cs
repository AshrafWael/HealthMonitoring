using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.APIRequst;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;
using HealthMonitoring.DAL.Data.Models;

namespace HealthMonitoring.BLL.IServices
{
    public interface IEmergencyContactService
    {
        Task<EmergencyContactResponseDto> CreateEmergencyContactAsync(CreateEmergencyContactDto createDto);
        Task<EmergencyContactResponseDto> CreateEmergencyContactAsync(string userid, CreateEmergencyContactDto createDto);
        Task<IEnumerable<EmergencyContactDto>> GetContactsByUserIdAsync(string userId);
        Task<IEnumerable<EmergencyContactDto>> GetContactsByUserEmailAsync(string email);
        Task<IEnumerable<ConectedUserDto>> GetUsersByContactIdAsync(int contactId);
        Task<bool> ConnectUserToContactAsync(string userEmail, int contactId);
        Task<bool> DisconnectUserFromContactAsync(string userId, int contactId);
        Task<EmergencyContactWithUsersDto> GetContactWithUsersAsync(int contactId);
    }
}

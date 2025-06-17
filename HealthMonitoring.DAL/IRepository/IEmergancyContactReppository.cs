using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;

namespace HealthMonitoring.DAL.IRepository
{
    public interface IEmergancyContactReppository :IBaseRepository<EmergencyContact>
    {
        Task AddContactToUserAsync(string userId, EmergencyContact contact);
        Task<List<ApplicationUser>> GetUsersByContactEmailAsync(string email);
        Task<IEnumerable<EmergencyContact>> GetContactsByUserIdAsync(string userId);
        Task<IEnumerable<EmergencyContact>> GetContactsByUserEmailAsync(string email);
        Task<EmergencyContact> GetContactWithUsersAsync(int contactId);
        public  Task<bool> ContactExistsByEmailAsync(string email);
        public Task<EmergencyContact> CreateEmergencyContactForUserAsync(string userid, EmergencyContact createcontact);
        //Task<IEnumerable<EmergencyContact>> GetContactsByUserIdAsync(string userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsForUserAsync(string phoneNumber, string userId);
    }
}

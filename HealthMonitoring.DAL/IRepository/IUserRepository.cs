using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;

namespace HealthMonitoring.DAL.IRepository
{
    public interface IUserRepository :IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser> FindUserAsync(Expression<Func<ApplicationUser, bool>> criteria);
        Task<IEnumerable<ApplicationUser>> GetUsersByEmergencyContactIdAsync(int contactId);
        Task<ApplicationUser> GetUserWithEmergencyContactsAsync(string userId);
        Task<ApplicationUser> GetByEmailAsync(string email);
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(HealthMonitoringContext dbcontext) : base(dbcontext) { }
        public async Task<ApplicationUser> FindUserAsync(Expression<Func<ApplicationUser, bool>> criteria)
        {
            try
            {
                IQueryable<ApplicationUser> query = _dbset;
                return await query.FirstOrDefaultAsync(criteria);
            }
            catch (Exception ex)
            {
                throw new ("Error finding user with specified criteria", ex);
            }
      
    }
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _dbcontext.ApplicationUsers
                .Include(u => u.EmergencyContacts)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> GetUserWithEmergencyContactsAsync(string userId)
        {
            return await _dbcontext.ApplicationUsers
                .Include(u => u.EmergencyContacts)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByEmergencyContactIdAsync(int contactId)
        {
            return await _dbcontext.ApplicationUsers
                .Where(u => u.EmergencyContacts.Any(ec => ec.ContactId == contactId))
                .ToListAsync();
        }

       


    }
}

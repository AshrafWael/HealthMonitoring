using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.DbHelper;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Repository
{
    public class EmergancyContactReppository : BaseRepository<EmergencyContact>, IEmergancyContactReppository
    {
       
    
        private readonly HealthMonitoringContext _dbcontext;

        public EmergancyContactReppository(HealthMonitoringContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));

        }


        public async Task AddContactToUserAsync(string userId, EmergencyContact contact)
        {
            var user = await _dbcontext.ApplicationUsers
              .Include(u => u.EmergencyContacts)
              .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception($"User with ID {userId} not found.");

            if (contact == null)
                throw new Exception("Emergency contact is null.");

            // Optional: check if contact already exists (by email or phone)
            user.EmergencyContacts.Add(contact);
            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine($"Contact Name: {contact?.Name}, Email: {contact?.Email}");
        }
        public async Task<List<ApplicationUser>> GetUsersByContactEmailAsync(string email)
        {
            // Add null checks and validation
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await _dbcontext.EmergencyContacts
                .Where(c => c.Email == email)
                .SelectMany(c => c.ApplicationUsers)
                .ToListAsync();
        }
        public async Task<IEnumerable<EmergencyContact>> GetContactsByUserIdAsync(string userId)
        {
            // Add null checks
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));

            return await _dbcontext.EmergencyContacts
                .Where(ec => ec.ApplicationUsers.Any(u => u.Id == userId))
                .ToListAsync();
        }
        //public async Task<IEnumerable<EmergencyContact>> GetContactsByUserIdAsync(string userId)
        //{
        //    return await _dbcontext.EmergencyContacts
        //        .Where(ec => ec.ApplicationUsers.Any(u => u.Id == userId))
        //        .Include(ec => ec.ApplicationUsers)
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<EmergencyContact>> GetContactsByUserEmailAsync(string email)
        {
            // Add null checks and proper error handling
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            try
            {
                return await _dbcontext.EmergencyContacts
                    .Where(ec => ec.ApplicationUsers.Any(u => u.Email == email))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                throw new InvalidOperationException($"Error retrieving contacts for user email: {email}", ex);
            }
        }
        public async Task<EmergencyContact> GetContactWithUsersAsync(int contactId)
        {
            if (contactId <= 0)
                throw new ArgumentException("ContactId must be greater than 0", nameof(contactId));

            return await _dbcontext.EmergencyContacts
                .Include(ec => ec.ApplicationUsers)
                .FirstOrDefaultAsync(ec => ec.ContactId == contactId);
        }
        public async Task<bool> ContactExistsByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _dbcontext.EmergencyContacts
                .AnyAsync(c => c.Email == email);
        }
        public async Task<EmergencyContact> CreateEmergencyContactForUserAsync(string userid ,EmergencyContact createcontact)
        {
            // Get the user
            var user = await _dbcontext.ApplicationUsers.FindAsync(userid);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userid} not found");
            }
            try
            {
                // Create emergency contact
                var emergencyContact = new EmergencyContact
                {
                    Name = createcontact.Name,
                    PhoneNumber = createcontact.PhoneNumber,
                    Email = createcontact.Email,
                    Relationship = createcontact.Relationship,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                // Add to context
                await _dbcontext.EmergencyContacts.AddAsync(emergencyContact);
                await _dbcontext.SaveChangesAsync();
                // Link with user
                emergencyContact.ApplicationUsers.Add(user);
                await _dbcontext.SaveChangesAsync();
                // Return with loaded navigation properties
                return await _dbcontext.EmergencyContacts
                    .Include(ec => ec.ApplicationUsers)
                    .FirstAsync(ec => ec.ContactId == emergencyContact.ContactId);
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbcontext.EmergencyContacts
                .AnyAsync(ec => ec.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> PhoneExistsForUserAsync(string phoneNumber, string userId)
        {
            return await _dbcontext.EmergencyContacts
                .AnyAsync(ec => ec.PhoneNumber == phoneNumber &&
                               ec.ApplicationUsers.Any(u => u.Id == userId));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models.AIModels;
using Microsoft.AspNetCore.Identity;

namespace HealthMonitoring.DAL.Data.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address   { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string? HealthGoals { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        //Navigation Proprty
        public ICollection<EmergencyContact> EmergencyContacts { get; set; }
        public ICollection<ActivityData> activityDatas { get; set; }
        public ICollection<HealthInformation> HealthInformation { get; set; }
        public ICollection<HeartRateData> HeartRateDatas  { get; set; }
        public ICollection<HealthSuggestion> HealthSuggestions { get; set; }   
        public ICollection<Notification> Notifications  { get; set; }
        public ICollection<BloodPressureReading> bloodPressureReadings { get; set; }
    }
}

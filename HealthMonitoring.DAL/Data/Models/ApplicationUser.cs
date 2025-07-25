﻿using System;
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
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public double? WeightKg { get; set; }
        public int? Height { get; set; }
        public string? HealthGoals { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        //Navigation Proprty
        public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
        public ICollection<ActivityData> activityDatas { get; set; }
        public  ICollection<CaloriesPrediction> CaloriesPredictions { get; set; }
        public ICollection<HeartDisease> HealthInformation { get; set; }
        public ICollection<HeartRateData> HeartRateDatas  { get; set; }
        public ICollection<BloodPressureReading> bloodPressureReadings { get; set; }
    }
}

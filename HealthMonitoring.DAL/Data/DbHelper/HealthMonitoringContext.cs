﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Configrations;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.DAL.Data.DbHelper
{
    public class HealthMonitoringContext :IdentityDbContext<ApplicationUser>
    {

        public HealthMonitoringContext(DbContextOptions<HealthMonitoringContext> dbContext) : base(dbContext) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ActivityData> ActivityDatas { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<HealthSuggestion> HealthSuggestions { get; set; }
        public DbSet<HeartRateData> HeartRateDatas { get; set; }
        public DbSet<HealthInformation> HealthInformations { get; set; }
        public DbSet<MedicalNews> MedicalNewsDatas { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<SensorDataPoint>  sensorDataPoints { get; set; }
        public DbSet<BloodPressureReading>  bloodPressureReadings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationUserEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new ActivityDataEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new EmergencyContactEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new HealthInformationEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new HealthSuggestionsEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new HeartRateDataEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new MedicalNewsEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new NotificationsEntityTypeConfigration());

/*
            modelBuilder.Entity<SensorDataPoint>()
       .Property(e => e.ECG)
       .HasConversion(
           v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
           v => JsonSerializer.Deserialize<List<double>>(v, new JsonSerializerOptions()));

            modelBuilder.Entity<SensorDataPoint>()
                .Property(e => e.PPG)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<double>>(v, new JsonSerializerOptions()));

            modelBuilder.Entity<SensorDataPoint>()
               .Property(e => e.ABP)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                   v => JsonSerializer.Deserialize<List<double>>(v, new JsonSerializerOptions()));
*/
            base.OnModelCreating(modelBuilder);

        }

    }
}

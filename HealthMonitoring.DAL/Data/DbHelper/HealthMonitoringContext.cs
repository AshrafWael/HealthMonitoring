using System;
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
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<ActivityData> ActivityDatas { get; set; }
        public DbSet<HeartRateData> HeartRateDatas { get; set; }
        public DbSet<BloodPressureReading>  bloodPressureReadings { get; set; } 
        public DbSet<HeartDisease> HeartDiseases { get; set; }
        public DbSet<SensorDataSet>  sensorDataSets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationUserEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new EmergencyContactEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new ActivityDataEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new HealthInformationEntityTypeConfigration());
            modelBuilder.ApplyConfiguration(new HeartRateDataEntityTypeConfigration());

            base.OnModelCreating(modelBuilder);

        }

    }
}

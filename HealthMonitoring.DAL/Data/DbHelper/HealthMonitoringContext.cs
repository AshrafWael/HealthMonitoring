using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Configrations;
using HealthMonitoring.DAL.Data.Models;
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
            base.OnModelCreating(modelBuilder);

        }

    }
}

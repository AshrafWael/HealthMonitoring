using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMonitoring.DAL.Configrations
{
    public class ApplicationUserEntityTypeConfigration : IEntityTypeConfiguration<ApplicationUser>
    {

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u=> u.activityDatas)
                   .WithOne(h=> h.User)
                   .HasForeignKey(ad => ad.UserId);

            builder.HasMany(u => u.EmergencyContacts)
                  .WithOne(h => h.User)
                  .HasForeignKey(EC => EC.UserId);

            builder.HasMany(u => u.HealthSuggestions)
                  .WithOne(h => h.User)
                  .HasForeignKey(hs => hs.UserId);

            builder.HasMany(u => u.HealthInformation)
                  .WithOne(h => h.User)
                  .HasForeignKey(hi => hi.UserId);

            builder.HasMany(u => u.HeartRateDatas)
                  .WithOne(h => h.User)
                  .HasForeignKey(hr => hr.UserId);

            builder.HasMany(u => u.Notifications)
                  .WithOne(h => h.User)
                  .HasForeignKey(n => n.UserId);

            builder.Property(u => u.FirstName)
                  
                   .HasMaxLength(50);
            builder.Property(u => u.LastName)
                  
                  .HasMaxLength(50);
            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(20);
         


        }
    }
}

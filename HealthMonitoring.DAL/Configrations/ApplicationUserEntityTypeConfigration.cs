﻿using System;
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

            builder.HasMany(u => u.EmergencyContacts)
                  .WithMany(h => h.ApplicationUsers);

            builder.HasMany(u=> u.activityDatas)
                   .WithOne(h=> h.User)
                   .HasForeignKey(ad => ad.UserId);

      
            builder.HasMany(u => u.HealthInformation)
                  .WithOne(h => h.User)
                  .HasForeignKey(hi => hi.UserId);

            builder.HasMany(u => u.HeartRateDatas)
                  .WithOne(h => h.User)
                  .HasForeignKey(hr => hr.UserId);


            builder.Property(u => u.FirstName)
                  
                   .HasMaxLength(50);
            builder.Property(u => u.LastName)
                  
                  .HasMaxLength(50);
            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(20);
         


        }
    }
}

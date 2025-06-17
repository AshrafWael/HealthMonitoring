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
    public class EmergencyContactEntityTypeConfigration : IEntityTypeConfiguration<EmergencyContact>
    {
        public void Configure(EntityTypeBuilder<EmergencyContact> builder)
        {

            builder.HasMany(u => u.ApplicationUsers)
                   .WithMany(H => H.EmergencyContacts);
            builder.Property(u => u.PhoneNumber)
                .IsRequired();
        }
    }
}



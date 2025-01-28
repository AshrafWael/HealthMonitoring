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
    public class EmergencyContactEntityTypeConfigration : IEntityTypeConfiguration<EmergencyContacts>
    {
        public void Configure(EntityTypeBuilder<EmergencyContacts> builder)
        {
            builder.Property(u => u.PhoneNumber)
                .IsRequired();

            builder.HasOne(u => u.User)
                   .WithMany(H => H.EmergencyContacts)
                   .HasForeignKey(u => u.UserId);

            builder.HasMany(u => u.notifications)
                   .WithOne(h => h.Contact)
                   .HasForeignKey(n => n.ContactId);
        }
    }
}



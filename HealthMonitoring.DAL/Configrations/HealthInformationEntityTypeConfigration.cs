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
    public class HealthInformationEntityTypeConfigration : IEntityTypeConfiguration<HealthInformation>
    {
        public void Configure(EntityTypeBuilder<HealthInformation> builder)
        {
            builder.Property(u => u.Diseases)
                            .IsRequired();

            builder.HasOne(u => u.User)
                   .WithMany(H => H.HealthInformation)
                   .HasForeignKey(u => u.UserId);
        }
    }
}

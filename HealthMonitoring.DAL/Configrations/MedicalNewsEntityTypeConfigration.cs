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
    public class MedicalNewsEntityTypeConfigration : IEntityTypeConfiguration<MedicalNews>
    {
        public void Configure(EntityTypeBuilder<MedicalNews> builder)
        {

            builder.Property(x => x.Title)
           .IsRequired()
           .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.Link)
                .HasMaxLength(500);

        }
    }
}

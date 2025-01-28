using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMonitoring.DAL.Configrations
{
    public class ActivityDataEntityTypeConfigration : IEntityTypeConfiguration<ActivityData>
    {
        public void Configure(EntityTypeBuilder<ActivityData> builder)
        {
            builder.Property(u => u.ActivityType)
                 .IsRequired();

            builder.HasOne(u => u.User)
                   .WithMany(H => H.activityDatas)
                   .HasForeignKey(u => u.UserId);
        }
    }
}

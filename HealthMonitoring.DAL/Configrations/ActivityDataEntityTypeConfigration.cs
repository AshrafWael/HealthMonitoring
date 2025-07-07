using HealthMonitoring.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Configrations
{
    public class ActivityDataEntityTypeConfigration : IEntityTypeConfiguration<ActivityData>
    {
        public void Configure(EntityTypeBuilder<ActivityData> builder)
        {
          

            builder.HasOne(d => d.User)
              .WithMany(u => u.activityDatas)
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

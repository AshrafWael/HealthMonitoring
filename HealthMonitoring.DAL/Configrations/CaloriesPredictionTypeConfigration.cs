using HealthMonitoring.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Configrations
{
    public class CaloriesPredictionTypeConfigration :IEntityTypeConfiguration<CaloriesPrediction>
    {
        public void Configure(EntityTypeBuilder<CaloriesPrediction> builder)
        {
            builder.HasOne(u => u.User)
                   .WithMany(H => H.CaloriesPredictions)
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(a => a.ActivityData)
                .WithMany()
                .HasForeignKey(a => a.ActivityDtataId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}

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
    public class HeartRateDataEntityTypeConfigration: IEntityTypeConfiguration<HeartRateData>
    {
        public void Configure(EntityTypeBuilder<HeartRateData> builder)
    {
            

            builder.HasOne(u => u.User)
                   .WithMany(H => H.HeartRateDatas)
                   .HasForeignKey(u => u.UserId);
        }
    
    }
}

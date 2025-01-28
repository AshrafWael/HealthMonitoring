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
           

         
        }
    }
}

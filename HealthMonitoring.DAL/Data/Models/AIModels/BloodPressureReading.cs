using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HealthMonitoring.DAL.Consts.StaticData;

namespace HealthMonitoring.DAL.Data.Models.AIModels
{
    public class BloodPressureReading
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double sbp { get; set; }
        public double dbp { get; set; }
        public BloodPressureCategory Category { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

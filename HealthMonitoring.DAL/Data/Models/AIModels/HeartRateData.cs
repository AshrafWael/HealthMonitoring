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
    public class HeartRateData
    {
        [Key]
        public int Id { get; set; }
        public DateTime RecordedAt { get; set; }
        public int HeartRate { get; set; }
        public HeartRateCategory Category { get; set; } 
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}

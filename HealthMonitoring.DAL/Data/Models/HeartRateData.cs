using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models
{
    public class HeartRateData
    {
        [Key]
        public int Id { get; set; }
        public DateTime RecordedAt { get; set; }
        public int HeartRate { get; set; }
        public string Status { get; set; } // normal or Hifh or Low 
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}

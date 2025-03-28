using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models.AIModels
{
    public class SensorDataSet
    {
        [Key]
        public int Id { get; set; }
        public double  ECG { get; set; }
        public double  ABP { get; set; }
        public double  PPG { get; set; }
        public DateTime Timestamp { get; set; }


        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Navigation property
       
    }
}

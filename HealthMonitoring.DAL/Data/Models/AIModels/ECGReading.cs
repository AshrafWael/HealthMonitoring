using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Data.Models.AIModels
{
    public class ECGReading
    {
        public string UserId { get; set; }
        public double ECG { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.BloodPressureDto
{
    public class BloodPressurReadDto
    {
        public DateTime Timestamp { get; set; }
        public double sbp { get; set; }
        public double dbp { get; set; }
        public string Category { get; set; }
    }
}

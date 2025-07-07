using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ActivityDataDtos
{
    public class UserActivityJsonDto
    {
        public Dictionary<string, double> TotalSteps { get; set; }
        public Dictionary<string, double> VeryActiveMinutes { get; set; }
        public Dictionary<string, double> FairlyActiveMinutes { get; set; }
        public Dictionary<string, double> LightlyActiveMinutes { get; set; }
        public Dictionary<string, double> SedentaryMinutes { get; set; }
        public Dictionary<string, double> TotalMinutesAsleep { get; set; }
        public Dictionary<string, double> WeightKg { get; set; }

    }
}

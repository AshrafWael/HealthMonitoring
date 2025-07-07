using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.BLL.Dtos.ActivityDataDtos
{
    public class DailyActivityDataDto
    {
        public int Day { get; set; }
        public double TotalSteps { get; set; }
        public double VeryActiveMinutes { get; set; }
        public double FairlyActiveMinutes { get; set; }
        public double LightlyActiveMinutes { get; set; }
        public double SedentaryMinutes { get; set; }
        public double TotalMinutesAsleep { get; set; }
        public double WeightKg { get; set; }
    }
}
